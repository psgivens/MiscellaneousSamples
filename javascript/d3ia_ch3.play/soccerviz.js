function createSoccerViz() {
  // d3.csv("worldcup.csv", function(data) {
  //   overallTeamViz(data);
  // })
  d3.csv("defects.csv", function(data) {
    overallTeamViz(data);
  })

  function summarizeData(data) {
    const severity = d1 => d1.severity;
    const team = d1 => d1.team;
    const state = d1 => d1.state;
    const rollupSum = d1 => d3.sum(d1, function(d2) { return d2.count; });

    const teamSums = d3.nest()
      .key(state)
      .key(severity)
      .key(team)
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

    const dataSums = summarizeData(incomingData);
    const severities = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.severity), new Set ()));

    const yScale = d3.scaleBand()
      .domain(severities)
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

      const labelMargin = 20;
      const xScale_State = d3.scaleLinear()
        .domain([ 0, 7 ])
        .range([ labelMargin, xScale_InterState.bandwidth()]);

      const xAxis = d3.axisTop()
        .scale(xScale_State)
        .tickSize(tickSize)
        .ticks(4);

      d3.selectAll("svg")
        .append("g")
        .attr("id","xAxis_" + d.key)
        .call(xAxis)
        .attr("transform","translate(" + xScale_InterState(d.key) + "," + (svgHeight ) + ")");

      // TODO: Place state label below xAxis

      // https://bl.ocks.org/EmbraceLife/677054c8f535c77ddd95485523d97fcd
      const chart = d3.selectAll("svg").append("g")
        .attr("id", "graph" + d.key);


      d.values.forEach(function(item){

        const xScale = value => xScale_State(value) - labelMargin;

        const barRegion = chart.selectAll("g."+item.Key)
          .data([item])
          .enter()
          .append("g")
          .attr("id", item.key)
          .attr("transform", d1 => "translate(" + xScale_InterState(d.key) + ", " + margin.top + ")");

        // Totals for the bar
        barRegion.append("text").text(d1 => d3.sum(d1.values, d2 => d2.value))
          .style("dominant-baseline", "central")
          .style("font-size", "14px")
          .attr("y", function(d1) { return yScale(d1.key) + yScale.bandwidth() / 2; })
          .attr("x", function(d1) { return 0; });

        let previous = 0;
        item.values.forEach(function(item1, index){
          // Subdivide the bar for a third vector
          const bar = barRegion
            .append("svg")
            .attr("y", d1 => yScale(d1.key))
            .attr("x", d1 => xScale(previous) + labelMargin)
            .attr("height", d1 => yScale.bandwidth())
            .attr("width", d1 => xScale(item1.value));

          // TODO: Color based on item.key or index
          bar.append("rect")
            .style("fill", "pink")
            .style("stroke", "black")
            .style("stroke-width", "1px")
            .attr("x", d1 => 0)
            .attr("class", "measurementRectangle")
            .attr("height", d1 => yScale.bandwidth())
            .attr("width", d1 => xScale(item1.value));


          bar.append("text")
            .text(d1 => ((item1.value) > 0) ? item1.value : "")
            .style("text-anchor", "middle")
            .style("dominant-baseline", "central")
            .style("fill", "black")
            .attr("y", "50%")
            .attr("x", "50%");

          previous += item1.value;

        })

      })
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
