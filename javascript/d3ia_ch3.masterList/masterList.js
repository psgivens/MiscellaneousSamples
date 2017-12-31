const topOffendersQuery = "https://data.lacounty.gov/resource/3te6-gtm8.json?&$query=" +
  "SELECT name,record_id,activity_date,sum(points) AS sum_points " +
  "GROUP BY name,record_id,activity_date " +
  "|> SELECT name,record_id,avg(sum_points) " +
  "GROUP BY name,record_id " +
  "ORDER BY avg_sum_points DESC " +
  "LIMIT 25 OFFSET 1";

const pointsPerDateQuery = "https://data.lacounty.gov/resource/3te6-gtm8.json?$query=" +
  "SELECT activity_date,record_id,name,score " +
  "WHERE record_id='{0}' " +
  "GROUP BY name,record_id,activity_date,score " +
  "ORDER BY activity_date"

const latestRecordForRestaurant = "https://data.lacounty.gov/resource/3te6-gtm8.json?$query=" +
  "SELECT activity_date,record_id,name,grade,score,location_1,site_zip " +
  "WHERE record_id='{0}' " +
  "ORDER BY activity_date DESC " +
  "LIMIT 1"

const averageScoreForYear = "https://data.lacounty.gov/resource/3te6-gtm8.json?$query=" +
  "SELECT record_id, activity_date, score, site_zip " +
  "WHERE starts_with(site_zip, '{0}') " +
  "AND activity_date > '{1}' " +
  "GROUP BY record_id, activity_date, site_zip, score " +
  "ORDER BY activity_date " +
  "|> SELECT avg(score) " +
  "GROUP BY site_zip"

const violationsInCategories = "https://data.lacounty.gov/resource/3te6-gtm8.json?$query=" +
  "SELECT sum(points) " +
  "WHERE violation_code IN ({1}) " +
  "AND record_id='{0}' " +
  "AND activity_date='{2}' " +
  "GROUP BY record_id"

const empHealthCodes = ['16F001','16F052','16F002','16F003','16F004'];
const prevContamCodes = ['16F005','16F006'];
const timeAndTempCodes = ['16F007','16F008','16F009','16F010','16F011'];
const protContamCodes = ['16F012','16F013','16F014'];
const foodSourceCodes = ['16F015','16F016','16F017'];
const critRiskCodes = ['16F018','16F019','16F020','16F053','16F021','16F021','16F022','16F023'];

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

var empHealthPowerGauge = null;
var contaminationPowerGauge = null;
var timeAndTempPowerGauge = null;
var protContamPowerGauge = null;
var foodSourcePowerGauge = null;
var criticalRiskPowerGauge = null;
function createMasterList() {
  empHealthPowerGauge = startGauge('power-gauge-emp-health',12);
  contaminationPowerGauge = startGauge('power-gauge-contamination',6);
  timeAndTempPowerGauge = startGauge("power-gauge-timeAndTemp",20);
  protContamPowerGauge = startGauge("power-gauge-protect-contam",10);
  foodSourcePowerGauge = startGauge("power-gauge-food-source",8);
  criticalRiskPowerGauge = startGauge("power-gauge-critical-risk",20);

  d3.request(topOffendersQuery,
    function(jdata){
      const incomingData = JSON.parse(jdata.response);
      var record_id = getParameterByName('record_id');
      drawDetails(record_id ? record_id : incomingData[0].record_id);

      // Set up sizing constants
      const svgHeight = 400;
      const svgWidth = 500;
      const margin = {top: 20, right: 10, bottom: 50, left: 250};
      const width = svgWidth - margin.left - margin.right;
      const height = svgHeight - margin.top - margin.bottom;
      const catSize = height / 3;
      const labelOffset = catSize / 2 + margin.top;
      const tickSize = 10;
      const labelMargin = 30;
      const labelHeight = 30;

      // Get some high level data
      const restaurantNames = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.name), new Set ()));

      // Create the scales
      const yScale = d3.scaleBand()
        .domain(restaurantNames)
        .range([margin.top, height + margin.top])
        .padding(0.2)
        .round(true);

      const xScale = d3.scaleLinear()
        .domain([0, 40.0])
        .range([ margin.left, width + margin.left ]);

      // Define the axis functions
      const yAxis = d3.axisLeft()
        .scale(yScale)
        .tickSize(10)
        .ticks(4);

      const xAxis = d3.axisBottom()
        .scale(xScale)
        .tickSize(5)
        .ticks(10);

      // Start with globals for the graph.
      const masterSvg = d3.select("svg#master-list-graph");

      masterSvg
        .append("g")
        .attr("id","yAxisG")
        .call(yAxis)
        .attr("transform","translate(" + margin.left + ",0)");

      masterSvg
        .append("g")
        .attr("id","xAxisG")
        .call(xAxis)
        .attr("transform","translate(0," + (height + margin.top) + ")");

      const chart = masterSvg.append("g")

      const bars = chart.selectAll("rect.bars")
        .data(incomingData)
        .enter()
        .append("rect")
        .attr("class", "bars")
        .style("fill", "#BB8FCE")
        .style("stroke", "black")
        .style("stroke-width", "0px")
        .attr("x", xScale(0))
        .attr("y", d => yScale(d.name))
        .attr("width", d => xScale(d.avg_sum_points) - xScale(0))
        .attr("height", d => yScale.bandwidth());

      bars.on("click", function(d) {
        bars.style("fill", "#BB8FCE");
        const bar = d3.select(this);
        bar.style("fill","red");

      drawDetails(d.record_id);
    });
  });
}

function healthCodeQuery(record_id, activity_date, healthCodes){
  const query = violationsInCategories
      .replace("{0}", record_id)
      .replace("{1}",
        healthCodes
          .map(w => "'{0}'".replace("{0}",w))
          .join(','))
      .replace("{2}", activity_date);
  return query;
}

function requestGaugeUpdate(record_id, activity_date, healthCodes, healthGauge){
  d3.request(healthCodeQuery(record_id, activity_date, healthCodes),
    function(jdata){
      const incomingData = JSON.parse(jdata.response);
      healthGauge.update(incomingData.length > 0 ? incomingData[0].sum_points: 0);
    }
  );
}

function drawSpecifics(record){
    empHealthPowerGauge.update(0);
    contaminationPowerGauge.update(0);
    timeAndTempPowerGauge.update(0);
    protContamPowerGauge.update(0);
    foodSourcePowerGauge.update(0);
    criticalRiskPowerGauge.update(0);

    if (record){
      const activity_date = record.activity_date;
      const record_id = record.record_id;
      record.selected = true;
      requestGaugeUpdate(record_id,activity_date,empHealthCodes,empHealthPowerGauge);
      requestGaugeUpdate(record_id,activity_date,prevContamCodes,contaminationPowerGauge);
      requestGaugeUpdate(record_id,activity_date,timeAndTempCodes,timeAndTempPowerGauge);
      requestGaugeUpdate(record_id,activity_date,protContamCodes,protContamPowerGauge);
      requestGaugeUpdate(record_id,activity_date,foodSourceCodes,foodSourcePowerGauge);
      requestGaugeUpdate(record_id,activity_date,critRiskCodes,criticalRiskPowerGauge);
    }
}

function updateRestaurantInfo(record){
  document.getElementById("restaurantId").textContent = record.record_id;
  document.getElementById("restaurantName").textContent = record.name;
  document.getElementById("restaurantName1").textContent = record.name;
  document.getElementById("restaurantName2").textContent = record.name;

  const score = document.getElementById("restaurantScore");
  const grade = document.getElementById("restaurantGrade");
  score.textContent = record.score;
  grade.textContent = record.grade;
  switch (record.grade) {
    case "A":
      score.style.color = 'green';
      grade.style.color = 'green';
      break;
    case "B":
      score.style.color = '#D68910';
      grade.style.color = '#D68910';
    break;case "C":
      score.style.color = 'red';
      grade.style.color = 'red';
      break;
    default:
      score.style.color = '#E74C3C';
      grade.style.color = '#E74C3C';
      break;

  }

  document.getElementById("restaurantAddress").textContent = record.location_1;
  document.getElementById("restaurantCity").textContent = record.site_city;
  document.getElementById("restaurantZip").textContent = record.site_zip;
  document.getElementById("restaurantLastInsp").textContent = new Date(record.activity_date).toDateString();
}

function drawDetails(record_id) {
  // Set up sizing constants
  const svgHeight = 400;
  const svgWidth = 500;
  const margin = {top: 20, right: 10, bottom: 50, left: 50};
  const height = svgHeight - margin.top - margin.bottom;
  const catSize = height / 3;

  const config = {
    svgHeight: 400,
    svgWidth: 500,
    margin: {top: 20, right: 10, bottom: 50, left: 20},
    width: svgWidth - margin.left - margin.right,
    height: svgHeight - margin.top - margin.bottom,
    catSize: height / 3,
    labelOffset: catSize / 2 + margin.top,
    tickSize: 10,
    labelMargin: 30,
    labelHeight: 30
  }

  const yScale = d3.scaleLinear()
    .domain([50, 100])
    .range([config.height + config.margin.top, config.margin.top]);

  // Define the axis functions
  const yAxis = d3.axisLeft()
    .scale(yScale)
    .tickSize(-config.width)
    .ticks(9);

    d3.selectAll("#averageLine").remove();
  const detailsSvg = d3.select("svg#detail-graph");

  detailsSvg
    .append("g")
    .attr("id","yAxisG")
    .call(yAxis)
    .attr("transform","translate(" + config.margin.left + ",0)")
    .selectAll("g.tick > line")
    .attr("opacity", "0.1")


  detailsSvg.append('line')
    .attr('class','dashed')
    .attr('x1', config.margin.left)
    .attr('y1', yScale(90))
    .attr('x2', config.margin.left + config.width)
    .attr('y2', yScale(90))
    .attr("stroke-width", 2)
    .attr("stroke", "black");

  detailsSvg.append('line')
    .attr('class','dashed')
    .attr('x1', config.margin.left)
    .attr('y1', yScale(80))
    .attr('x2', config.margin.left + config.width)
    .attr('y2', yScale(80))
    .attr("stroke-width", 2)
    .attr("stroke", "black");

  detailsSvg.append('line')
   .attr('class','dashed')
   .attr('x1', config.margin.left)
   .attr('y1', yScale(70))
   .attr('x2', config.margin.left + config.width)
   .attr('y2', yScale(70))
   .attr("stroke-width", 2)
   .attr("stroke", "black");

  d3.request(pointsPerDateQuery.replace("{0}", record_id),
    function(jdata){
      const incomingData = JSON.parse(jdata.response);

      if (incomingData.length > 0){
        const selectedRecord = incomingData[incomingData.length - 1];
        updateRestaurantInfo(selectedRecord);
        drawSpecifics(selectedRecord);

        d3.request(latestRecordForRestaurant.replace("{0}", record_id),
          function(recordJson){
            const fullRecords = JSON.parse(recordJson.response);
            const fullRecord = fullRecords[0];
            updateRestaurantInfo(fullRecord);
            const yearAgo = new Date(new Date().setFullYear(new Date().getFullYear() - 1)).toISOString().replace('Z','') ;

            d3.request(averageScoreForYear
              .replace("{0}", fullRecord.site_zip.substring(0,5))
              .replace("{1}",yearAgo),
                function(averageJson){
                  const averageArray = JSON.parse(averageJson.response);
                  const yearAverage = averageArray[0].avg_score;
                  detailsSvg.append('line')
                    .attr('id','averageLine')
                    .attr('x1', config.margin.left)
                    .attr('y1', yScale(yearAverage))
                    .attr('x2', config.margin.left + config.width)
                    .attr('y2', yScale(yearAverage))
                    .attr("stroke-width", 2)
                    .attr("stroke", "purple");

                })

          })

      }

      // Get some high level data
      const activityDates = Array.from(incomingData.reduce((acc,d1) => acc.add((new Date(d1.activity_date)).toDateString()), new Set ()));

      // Create the scales
      const xScale = d3.scaleBand()
        .domain(activityDates)
        .range([ config.margin.left, config.width + config.margin.left ])
        .padding(1);

      const xAxis = d3.axisBottom()
        .scale(xScale)
        .tickSize(5)
        .ticks(activityDates.length);

      detailsSvg.select("#xAxisG").remove();
      detailsSvg
        .append("g")
        .attr("id","xAxisG")
        .call(xAxis)
        .attr("transform","translate(0," + (config.height + config.margin.top) + ")")
        .selectAll("text")
        .attr("x","10")
        .attr("y","-8")
        .attr("transform", "rotate(90)")
        .style("text-anchor", "start")
        .style("dominant-baseline", "central");

     const instanceLine = d3.line()
        .x(d => xScale((new Date(d.activity_date)).toDateString()))
        .y(d => yScale(d.score));

     detailsSvg
      .selectAll("path.instanceLine")
      .remove();

     detailsSvg
      .append("path")
      .attr("class", "instanceLine")
      .attr("d", instanceLine(incomingData))
      .attr("fill", "none")
      .attr("stroke", "#fe9a22")
      .attr("stroke-width", 2);

      detailsSvg.selectAll("circle.point").remove ();
      const circles = detailsSvg.selectAll("circle.point")
        .data(incomingData)
        .enter()
       .append("circle")
       .attr("class", "point")
       .attr("r", 7)
       .attr("cx", d => xScale(new Date(d.activity_date).toDateString()))
       .attr("cy", d => yScale(d.score))
       .style("fill", d => d.selected ? "purple" : "blue");

      circles.on("click", function(d) {
        incomingData.forEach(record => record.selected = false);
        drawSpecifics(d);
        circles.style("fill", d => d.selected ? "purple" : "blue")
      });

  }
)};
