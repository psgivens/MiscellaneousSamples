function createSoccerViz() {
  // d3.csv("worldcup.csv", function(data) {
  //   overallTeamViz(data);
  // })
  d3.csv("defects.csv", function(data) {
    overallTeamViz(data);
  })

  function barId(severity, state){
      return "bar_" + severity + "_" + state;
  }

  function summarizeData(data) {
    const severity = d1 => d1.severity;
    const team = d1 => d1.team;
    const state = d1 => d1.state;
    const rollupSum = d1 => d3.sum(d1, function(d2) { return d2.count; });

    const teamSums = d3.nest()
      .key(state)
      .key(severity)
      // .key(team)
      .rollup(rollupSum)
      .entries(data);

    return teamSums;
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

    const categories = ["High", "Medium", "Low"]

    const dataSums = summarizeData(incomingData);
    console.log(dataSums);

    const yScale = d3.scaleBand()
      .domain(categories)
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

    dataSums.map(function(d, i) {

      const xScale_State = d3.scaleLinear()
        .domain([ 0, 7 ])
        .range([ 0, 0 + xScale_InterState.bandwidth() ]);

      const xAxis = d3.axisTop()
        .scale(xScale_State)
        .tickSize(tickSize)
        .ticks(4);

      d3.selectAll("svg")
        .append("g")
        .attr("id","xAxis_" + d.key)
        .call(xAxis)
        .attr("transform","translate(" + xScale_InterState(d.key) + "," + (svgHeight ) + ")");

      // https://bl.ocks.org/EmbraceLife/677054c8f535c77ddd95485523d97fcd
      const chart = d3.selectAll("svg").append("g")
        .attr("id", "graph" + d.key);

      const bar = chart.selectAll("g")
        .data(d.values)
        .enter().append("g")
        .attr("transform", function(d1) { return "translate(" + xScale_InterState(d.key) + ", " + margin.top + ")"; });

      bar.append("rect")
        .style("fill", "pink")
        .style("stroke", "black")
        .style("stroke-width", "1px")
        .attr("id", function(d1) { return barId(d1.severity,d1.state);})
        .attr("class", "measurementRectangle")
        .attr("y", function(d1) { return yScale(d1.key); })
        .attr("x", function(d1) { return 0; })
        .attr("height", function(d1) { return yScale.bandwidth(); })
        .attr("width", function(d1) { return xScale_State(d1.value); });
    });

    const mRects = d3.selectAll("rect.measurementRectangle");
    mRects.on("mouseover", function(d) {
      const bar = d3.select(this);
      bar.style("fill","red");
    });

    mRects.on("mouseout", function(d) {
      const bar = d3.select(this);
      bar.style("fill","pink");
    });

  }
}
