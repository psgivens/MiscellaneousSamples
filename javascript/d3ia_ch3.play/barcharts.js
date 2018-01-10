function createBarCharts() {
  d3.csv("defects.csv", function(data) {
    overallTeamViz(data);
  })
  function summarizeData(data, teams) {
    const teamSums = d3.nest()
      .key(d => d.state)
      .key(d => d.team)
      .key(d => d.source)
      .key(d => d.severity)
      .rollup(d => d3.sum(d, d1 => d1.count))
      .entries(data);

    const mapSource = source => {
        const severities = source.values;
        let offset = 0;
        severities.forEach(item => {
          item.x0 = offset;
          offset += item.value;
          item.x1 = offset;
        })
        return {
          key:source.key,
          values:severities,
          total:severities.reduce((acc,item)=>acc+item.value,0)
        }
      }

    const mapTeam = team => {
      return {
        key:team.key,
        values:team.values.map(mapSource),
        max:20, // TODO: Calculate max
        total:5
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
    const margin = {top: 20, right: 120, bottom: 50, left: 70};
    const width = svgWidth - margin.left - margin.right;
    const height = svgHeight - margin.top - margin.bottom;
    const catSize = height / 3;
    const labelOffset = catSize / 2 + margin.top;
    const tickSize = 10;
    const labelMargin = 30;
    const labelHeight = 30;

    // Munge the data
    const teams = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.team), new Set ()));
    const severities  = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.severity), new Set ()));
    const sources  = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.source), new Set ()));
    const dataSums = summarizeData(incomingData, teams);

    // Create the scales
    const yScale = d3.scaleBand()
      .domain(teams)
      .range([margin.top, svgHeight - margin.bottom])
      .padding(0.2)
      .round(true);

    const createSourceScale = (teamName) => {
      const teamTop = yScale(teamName);
      const teamBottom = teamTop + yScale.bandwidth();

      // Create the scales
      return d3.scaleBand()
        .domain(sources)
        .range([teamTop, teamBottom])
        .padding(0.2)
        .round(true);
    };

    const xScale_InterState = d3.scaleBand()
      .domain(["Back Log","In Progress", "Done"])
      .range([ margin.left, width + margin.left ])
      .padding(0.1)
      .round(true);

    const xScale_State = d3.scaleLinear()
      .domain([ 0, 15 ])
      .range([ labelMargin, xScale_InterState.bandwidth() - labelMargin]);

    // Define the axis functions
    const yAxis = d3.axisLeft()
      .scale(yScale)
      .tickSize(0)
      .ticks(4);

    const xAxis = d3.axisBottom()
      .scale(xScale_State)
      .tickSize(0-svgHeight + 20)
      .ticks(10);

    const svg = d3.selectAll("svg");

    // Start with globals for the graph.
    svg.append("g")
      .attr("id","yAxisG")
      .call(yAxis)
      .attr("transform","translate(" + margin.left + "," + margin.top + ")")
      .selectAll("path.domain")
      .attr("display", "none");

    // TODO: Add legend
    // d3.select("svg")
    //   .selectAll("svg.legend")
    //   .append("svg")
    //   .attr("transform", "translate(" + (width + margin.left) + ",100")
    //   .attr("class", "legend")
    //   .data(severities)
    //   .enter()
    //   .append("svg")
    //   .append("text")
    //   .text("Legend")
    //   .attr("y", "100")
    //   // .attr("x", "200")

    // Create the chart for each 'category'
    const charts = svg.selectAll("g.stackedBars")
      .data(dataSums)
      .enter()
      .append("g")
      .attr("class", "stackedBars")
      .attr("id", d => "xAxis_" + d.state)
      .attr("width", xScale_InterState.bandwidth());

    charts.each(function(d,i){
      const chart = d3.select(this);

      const xa = chart
        .append("g")
        .call(xAxis)
        .attr("transform", "translate(" + xScale_InterState(d.state) + "," + svgHeight + ")");

      xa.selectAll("path.domain")
        .attr("opacity", "0.2");

      xa.selectAll("g.tick > line")
        .attr("opacity","0.2");

      chart.append("svg")
        .attr("class", "axisLabel")
        .attr("width", xScale_InterState.bandwidth())
        .attr("height", labelHeight)
        .attr("x", xScale_InterState(d.state) )
        .attr("y", (svgHeight + 10))
        .append("text")
        .text(d => d.state)
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .style("font-size", "14px")
        .attr("y", "50%")
        .attr("x", "50%");

      // Create a barRegion for each 'team'
      const barRegions = chart.selectAll("svg.state")
        .data(d.teams)
        .enter()
        .append("svg")
        .attr("id", d => d.state)
        .attr("class", "state")
        .attr("x", xScale_InterState(d.state))
        .attr("y", margin.top);

      barRegions.each(function(team,i1){
        const teamBarRegion = d3.select(this);
        const yScale_Source = createSourceScale(team.key);

        teamBarRegion.selectAll("svg.source_total")
          .data(team.values)
          .enter()
          .append("svg")
          .attr("class", "source_total")
          .attr("y", d1 => yScale_Source(d1.key))
          .attr("x", "0")
          .attr("width", labelMargin)
          .attr("height", yScale_Source.bandwidth())
          .append("text")
          .text(d1 => d1.total)
          .attr("x", "50%")
          .attr("y", "50%")
          .style("dominant-baseline", "central")
          .style("font-size", "10px");

        // Create a bar segment for each 'severity'
        const bars = teamBarRegion.selectAll("svg.bars")
          .data(team.values)
          .enter()
          .append("svg")
          .attr("class", "bars")
          .attr("x", d1 => xScale_State(0))
          .attr("y", d1 => yScale(team.key))
          // .attr("class", "measurementRectangle")
          .attr("height", d1 => yScale.bandwidth())
          .attr("logs", d1 => console.log(xScale_State(d1.total)))
          .attr("width", d1 => xScale_State(d1.total) - labelMargin);

        bars.each(function(source,i2){
          const bar = d3.select(this);
          // data: source


          // TODO: Adjust bar to make this sizing work.
          // bar.append("text")
          //   .text(source.total)
          //   .style("dominant-baseline", "central")
          //   .style("font-size", "14px")
          //   .attr("y", yScale_Source(source.key) + yScale_Source.bandwidth() / 2)
          //   .attr("x", "0");

          // Create a bar segment for each 'severity'
          const sourceBars = teamBarRegion.selectAll("svg.source_bars")
            .data(source.values)
            .enter()
            .append("svg")
            .attr("class", "source_bars")
            .attr("x", d1 => xScale_State(d1.x0))
            .attr("y", d1 => yScale_Source(source.key))
            .attr("class", "measurementRectangle")
            .attr("height", d1 => yScale_Source.bandwidth())
            .attr("width", d1 => xScale_State(d1.value) - labelMargin);

          sourceBars.append("rect")
            .style("stroke", "black")
            .style("stroke-width", "1px")
            .attr("x","0")
            .attr("y","0")
            .attr("class", d => "measurementRectangle " + d.key)
            .attr("height", d1 => yScale.bandwidth())
            .attr("width", d1 => xScale_State(d1.value) - labelMargin);


          sourceBars.append("text")
            .text(d1 => d1.value)
            .style("text-anchor", "middle")
            .style("dominant-baseline", "central")
            .style("fill", "black")
            .attr("y", "50%")
            .attr("x", "50%");

          });

        });
    //   });
    // })
    //
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
  })
}
}
