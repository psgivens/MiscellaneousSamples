const topOffendersQuery = "https://data.lacounty.gov/resource/3te6-gtm8.json?&$query=" +
      "SELECT name,record_id,activity_date,sum(points) AS sum_points " +
      "GROUP BY name,record_id,activity_date " +
      "|> SELECT name,record_id,avg(sum_points) " +
      "GROUP BY name,record_id " +
      "ORDER BY avg_sum_points DESC " +
      "LIMIT 10";

const pointsPerDateQuery = "https://data.lacounty.gov/resource/3te6-gtm8.json?$query=" +
      "SELECT activity_date,record_id,name,score " +
      "WHERE record_id='{0}' " +
      "GROUP BY name,record_id,activity_date,score " +
      "ORDER BY activity_date"

const violationsInCategories = "https://data.lacounty.gov/resource/3te6-gtm8.json?$query=" +
  "SELECT count(points) " +
  "WHERE violation_code IN ('16F013', '16F014', '16F016', '16F052', '16F052', '16F022', '16F023', '16F023') " +
  "AND record_id='{0}' " +
  "GROUP BY record_id"

function createMasterList() {
  startGuage();
  d3.request(topOffendersQuery,
    function(jdata){
      const incomingData = JSON.parse(jdata.response);
      drawDetails(incomingData[0].record_id);

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
        .style("fill", "pink")
        .style("stroke", "black")
        .style("stroke-width", "1px")
        .attr("x", xScale(0))
        .attr("y", d => yScale(d.name))
        .attr("width", d => xScale(d.avg_sum_points) - xScale(0))
        .attr("height", d => yScale.bandwidth());

      bars.on("click", function(d) {
        bars.style("fill", "pink");
        const bar = d3.select(this);
        bar.style("fill","red");

      drawDetails(d.record_id);
    });
  });
}

function drawDetails(record_id) {
  d3.request(violationsInCategories.replace("{0}", record_id),
    function(jdata){
      const incomingData = JSON.parse(jdata.response);
      console.log("++++ ")
      console.log(incomingData)
      powerGauge.update(incomingData[0].count_points);
    }
  );
  d3.request(pointsPerDateQuery.replace("{0}", record_id),
    function(jdata){

      const incomingData = JSON.parse(jdata.response);
      console.log(incomingData);

      // Set up sizing constants
      const svgHeight = 400;
      const svgWidth = 500;
      const margin = {top: 20, right: 10, bottom: 50, left: 50};
      const width = svgWidth - margin.left - margin.right;
      const height = svgHeight - margin.top - margin.bottom;
      const catSize = height / 3;
      const labelOffset = catSize / 2 + margin.top;
      const tickSize = 10;
      const labelMargin = 30;
      const labelHeight = 30;

      // Get some high level data
      const activityDates = Array.from(incomingData.reduce((acc,d1) => acc.add((new Date(d1.activity_date)).toDateString()), new Set ()));

      // Create the scales
      const xScale = d3.scaleBand()
        .domain(activityDates)
        .range([ margin.left, width + margin.left ])
        .padding(1);

      const yScale = d3.scaleLinear()
        .domain([0, 100])
        .range([height + margin.top, margin.top]);

      // Define the axis functions
      const yAxis = d3.axisLeft()
        .scale(yScale)
        .tickSize(10)
        .ticks(4);

      const xAxis = d3.axisBottom()
        .scale(xScale)
        .tickSize(5)
        .ticks(activityDates.length);

      const detailsSvg = d3.select("svg#detail-graph");

      detailsSvg
        .append("g")
        .attr("id","yAxisG")
        .call(yAxis)
        .attr("transform","translate(" + margin.left + ",0)");

      detailsSvg.select("#xAxisG").remove();
      detailsSvg
        .append("g")
        .attr("id","xAxisG")
        .call(xAxis)
        .attr("transform","translate(0," + (height + margin.top) + ")")
        .selectAll("text")
        .attr("x","10")
        .attr("y","-8")
        .attr("transform", "rotate(90)")
        .style("text-anchor", "start")
        .style("dominant-baseline", "central")

      detailsSvg.selectAll("circle.point").remove ();
      detailsSvg.selectAll("circle.point")
        .data(incomingData)
        .enter()
       .append("circle")
       .attr("class", "point")
       .attr("r", 5)
       .attr("cx", d => xScale(new Date(d.activity_date).toDateString()))
       .attr("cy", d => yScale(d.score))
       .style("fill", "blue")

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
  }
)};
