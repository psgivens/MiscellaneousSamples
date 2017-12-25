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

    const dataByState = d3.nest()
      .key(d => d.state)
      .entries(incomingData);

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

    dataByState.map(function(d, i) {

      // TODO: Build xScale per category
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

      var tempData = d.values.reduce((acc,o) => {acc[o.severity] = o; return acc}, {});

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
        .attr("y", function(d) { return yScale(d.severity); })
        .attr("x", function(d) { return 0; })
        .attr("height", function(d) { return yScale.bandwidth(); })
        .attr("width", function(d) { return xScale_State(d.count); });
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
