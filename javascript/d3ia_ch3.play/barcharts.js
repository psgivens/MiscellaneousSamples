function createBarCharts() {
  // d3.csv("worldcup.csv", function(data) {
  //   overallTeamViz(data);
  // })
  d3.csv("defects.csv", function(data) {
    overallTeamViz(data);
  })

const colors = ["#C0392B", "#E59866", "#FCF3CF"];

  function summarizeData(data) {
    const severity = d1 => d1.severity;
    const team = d1 => d1.team;
    const state = d1 => d1.state;
    const rollupSum = d1 => d3.sum(d1, d2 =>  d2.count);

    const teamSums = d3.nest()
      .key(state)
      .key(team)
      .key(severity)
      .rollup(rollupSum)
      .entries(data);

    const teams = Array.from(data.reduce((acc,d1) => acc.add(d1.team), new Set ()));

    const mapTeam = team => {
      const severities = team.values;
      let offset = 0;
      severities.forEach(item => {
        item.x0 = offset;
        offset += item.value;
        item.x1 = offset;
      })
      return {
        key:team.key,
        values:severities,
        total:severities.reduce((acc,item)=>acc+item.value,0)
      }
    }

    const teamSumsR = teamSums
      .map(stateItem => {
        return {
          state:stateItem.key,
          teams:stateItem.values.map(mapTeam)
        }
      });

    return teamSumsR;
  }

  function overallTeamViz(incomingData) {

    const svgHeight = 200;
    const svgWidth = 900;
    const margin = {top: 20, right: 80, bottom: 50, left: 80};
    const width = svgWidth - margin.left - margin.right;
    const height = svgHeight - margin.top - margin.bottom;
    const catSize = height / 3;
    const labelOffset = catSize / 2 + margin.top;
    const tickSize = 10;

    const dataSums = summarizeData(incomingData);
    const severities = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.severity), new Set ()));
    const teams = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.team), new Set ()));

    const yScale = d3.scaleBand()
      .domain(teams)
      .range([margin.top, svgHeight - margin.bottom])
      .padding(0.2)
      .round(true);

    const yaxis = d3.axisLeft()
      .scale(yScale)
      .tickSize(tickSize)
      .ticks(4);

    d3.selectAll("svg")
      .append("g")
      .attr("id","yAxisG")
      .call(yaxis)
      .attr("transform","translate(" + margin.left + "," + margin.top + ")");

    const xScale_InterState = d3.scaleBand()
      .domain(["BL","IP", "Done"])
      .range([ margin.left, width + margin.left ])
      .padding(0.1)
      .round(true);

    const labelMargin = 30;

    const xScale_State = d3.scaleLinear()
      .domain([ 0, 15 ])
      .range([ labelMargin, xScale_InterState.bandwidth() - labelMargin]);

    const xAxis = d3.axisTop()
      .scale(xScale_State)
      .tickSize(tickSize)
      .ticks(4);

    const charts = d3.select("svg")
      .selectAll("g.stackedBars")
      .data(dataSums)
      .enter()
      .append("g")
      .attr("class", "stackedBars")
      .attr("id", d => "xAxis_" + d.state)
      .attr("width", xScale_InterState.bandwidth())
      .attr("x",d => "translate(" + xScale_InterState(d.state));

    charts.each(function(d,i){
      const chart = d3.select(this);

      const xAxis = d3.axisTop()
        .scale(xScale_State)
        .tickSize(tickSize)
        .ticks(4);

      chart
        .append("g")
        .call(xAxis)
        .attr("transform", "translate(" + xScale_InterState(d.state) + "," + svgHeight + ")");

      const barRegions = chart.selectAll("g."+d.state)
        .data(d.teams)
        .enter()
        .append("g")
        .attr("id", d => d.state)
        .attr("transform", d1 => "translate(" + xScale_InterState(d.state) + ", " + margin.top + ")");

      barRegions.each(function(team,i1){
        const teamBarRegion = d3.select(this);
        teamBarRegion.append("text")
          .text(team.total)
          .style("dominant-baseline", "central")
          .style("font-size", "14px")
          .attr("y", yScale(team.key) + yScale.bandwidth() / 2)
          .attr("x", "0");

        const bars = teamBarRegion.selectAll("svg.bars")
          .data(team.values)
          .enter()
          .append("svg")
          .attr("class", "bars")
          .attr("x", d1 => xScale_State(d1.x0))
          .attr("y", d1 => yScale(team.key))
          .attr("class", "measurementRectangle")
          .attr("height", d1 => yScale.bandwidth())
          .attr("width", d1 => xScale_State(d1.value) - labelMargin);

        bars.append("rect")
          .style("fill", (d1,i1) => colors[i1])
          .style("stroke", "black")
          .style("stroke-width", "1px")
          .attr("x","0")
          .attr("y","0")
          .attr("class", "measurementRectangle")
          .attr("height", d1 => yScale.bandwidth())
          .attr("width", d1 => xScale_State(d1.value) - labelMargin);

        bars.append("text")
          .text(d1 => d1.value)
          .style("text-anchor", "middle")
          .style("dominant-baseline", "central")
          .style("fill", "black")
          .attr("y", "50%")
          .attr("x", "50%");

      });

    })

    const mRects = d3.selectAll("rect.measurementRectangle");
    mRects.on("mouseover", function(d) {
      const bar = d3.select(this);

      // hack
      bar.attr("temp", bar.style("fill"));
      bar.style("fill","red");

    });

    mRects.on("mouseout", function(d) {
      const bar = d3.select(this);
      // hack
      bar.style("fill",bar.attr("temp"));
    });
  }
}
