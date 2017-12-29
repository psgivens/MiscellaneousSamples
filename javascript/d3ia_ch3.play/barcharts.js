function createBarCharts() {
  d3.csv("defects.csv", function(data) {
    overallTeamViz(data);
  })
  function summarizeData(data, teams) {
    const teamSums = d3.nest()
      .key(d => d.state)
      .key(d => d.team)
      .key(d => d.severity)
      .rollup(d => d3.sum(d, d1 => d1.count))
      .entries(data);

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
      .map(stateItem => Object.create({
          state:stateItem.key,
          teams:stateItem.values.map(mapTeam)
        }));

    return teamSumsR;
  }

  function overallTeamViz(incomingData) {

    // Set up sizing constants
    const svgHeight = 200;
    const svgWidth = 900;
    const margin = {top: 20, right: 80, bottom: 50, left: 80};
    const width = svgWidth - margin.left - margin.right;
    const height = svgHeight - margin.top - margin.bottom;
    const catSize = height / 3;
    const labelOffset = catSize / 2 + margin.top;
    const tickSize = 10;
    const labelMargin = 30;
    const labelHeight = 30;

    // Munge the data
    const teams = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.team), new Set ()));
    const dataSums = summarizeData(incomingData, teams);

    // Create the scales
    const yScale = d3.scaleBand()
      .domain(teams)
      .range([margin.top, svgHeight - margin.bottom])
      .padding(0.2)
      .round(true);

    const xScale_InterState = d3.scaleBand()
      .domain(["BL","IP", "Done"])
      .range([ margin.left, width + margin.left ])
      .padding(0.1)
      .round(true);

    const xScale_State = d3.scaleLinear()
      .domain([ 0, 15 ])
      .range([ labelMargin, xScale_InterState.bandwidth() - labelMargin]);

    // Define the axis functions
    const yaxis = d3.axisLeft()
      .scale(yScale)
      .tickSize(tickSize)
      .ticks(4);

    const xAxis = d3.axisTop()
      .scale(xScale_State)
      .tickSize(tickSize)
      .ticks(4);

    // Start with globals for the graph.
    d3.selectAll("svg")
      .append("g")
      .attr("id","yAxisG")
      .call(yaxis)
      .attr("transform","translate(" + margin.left + "," + margin.top + ")");

    // TODO: Add legend

    // Create the chart for each 'category'
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

      chart
        .append("g")
        .call(xAxis)
        .attr("transform", "translate(" + xScale_InterState(d.state) + "," + svgHeight + ")");

      chart.append("svg")
        .attr("class", "axisLabel")
        .attr("width", xScale_InterState.bandwidth())
        .attr("height", labelHeight)
        .attr("transform", "translate(" + (xScale_InterState(d.state)) + "," + svgHeight + ")")
        .append("text")
        .text(d => d.state)
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .style("font-size", "14px")
        .attr("y", "50%")
        .attr("x", "50%");

      // Create a barRegion for each 'team'
      const barRegions = chart.selectAll("g.state")
        .data(d.teams)
        .enter()
        .append("g")
        .attr("id", d => d.state)
        .attr("class", "state")
        .attr("transform", d1 => "translate(" + xScale_InterState(d.state) + ", " + margin.top + ")");

      barRegions.each(function(team,i1){
        const teamBarRegion = d3.select(this);
        teamBarRegion.append("text")
          .text(team.total)
          .style("dominant-baseline", "central")
          .style("font-size", "14px")
          .attr("y", yScale(team.key) + yScale.bandwidth() / 2)
          .attr("x", "0");

        // Create a bar segment for each 'severity'
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
          .style("stroke", "black")
          .style("stroke-width", "1px")
          .attr("x","0")
          .attr("y","0")
          .attr("class", d => "measurementRectangle " + d.key)
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
      bar.style("fill","red");
    });

    mRects.on("mouseout", function(d) {
      const bar = d3.select(this);

      // Let the stylesheet take over
      bar.style("fill", "")
    });
  }
}
