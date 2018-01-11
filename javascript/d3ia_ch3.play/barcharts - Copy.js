function createBarCharts() {
  d3.csv("defects.csv", function(data) {
    overallTeamViz(data);
  })

  function formatData(data, categories) {
    const mapCat2 = cat2Item => {
        const cat3Items = cat2Item.values;
        let offset = 0;
        cat3Items.forEach(item => {
          item.cat3Key = item.key;
          item.x0 = offset;
          offset += item.value;
          item.x1 = offset;
        })
        return {
          cat2Key:cat2Item.key,
          cat3Values:cat3Items,
          total:cat3Items.reduce((acc,item)=>acc+item.value,0)
        }
      }

    const mapCat1 = cat1Item => {
      const cat2Items = cat1Item.values.map(mapCat2);
      return {
        cat1Key:cat1Item.key,
        cat2Values:cat2Items,
        max:cat2Items.reduce((acc,item)=>Math.max(acc,item.total))
      }
    }

    const nestedData = d3.nest()
      .key(d => d[categories[0]])
      .key(d => d[categories[1]])
      .key(d => d[categories[2]])
      .key(d => d[categories[3]])
      .rollup(d => d3.sum(d, d1 => d1.count))
      .entries(data);

    const formattedData = nestedData
      .map(cat0Item => Object.create({
          cat0Key:cat0Item.key,
          cat1Values:cat0Item.values.map(mapCat1)
        }));

    return formattedData;
  }

  function buildCat3Bars(_this, cat2BarRegion, scales, config) {
    return function(source,i2){
      const bar = d3.select(_this);

      // Create a bar segment for each 'severity'
      const sourceBars = cat2BarRegion.selectAll("svg.source_bars")
        .data(source.cat3Values)
        .enter()
        .append("svg")
        .attr("class", "source_bars")
        .attr("x", d1 => xScale_State(d1.x0))
        .attr("y", d1 => yScale_Source(source.cat2Key))
        .attr("class", "measurementRectangle")
        .attr("height", d1 => yScale_Source.bandwidth())
        .attr("width", d1 => xScale_State(d1.value) - labelMargin);

      sourceBars.append("rect")
        .style("stroke", "black")
        .style("stroke-width", "1px")
        .attr("x","0")
        .attr("y","0")
        .attr("class", d => "measurementRectangle " + d.cat3Key)
        .attr("height", d1 => yScale.bandwidth())
        .attr("width", d1 => xScale_State(d1.value) - labelMargin);

      sourceBars.append("text")
        .text(d1 => d1.value)
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .attr("y", "50%")
        .attr("x", "50%");



      };
    };
  }

  function overallTeamViz(incomingData) {

    /* Configure the graph with sizes.
    ************************/
    const svgHeight = 200;
    const svgWidth = 1200;
    const margin = {top: 20, right: 120, bottom: 50, left: 70};
    const width = svgWidth - margin.left - margin.right;
    const height = svgHeight - margin.top - margin.bottom;
    const catSize = height / 3;
    const labelOffset = catSize / 2 + margin.top;
    const tickSize = 10;
    const labelMargin = 30;
    const labelHeight = 30;
    const config = {svgHeight,svgWidth,margin,width,height,catSize,labelOffset,
                    tickSize,labelMargin,labelHeight};

    /* Get the categories
    ************************/
    const states = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.state), new Set ()));
    const teams = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.team), new Set ()));
    const severities  = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.severity), new Set ()));
    const sources  = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.source), new Set ()));
    const meta = {states,teams,severities,sources};

    /* Munge the data
    ************************/
    const dataSums = formatData(incomingData, ["state","team","source","severity"]);

    // Create the scales
    const yScale = d3.scaleBand()
      .domain(teams)
      .range([margin.top, svgHeight - margin.bottom])
      .padding(0.00)
      .round(true);

    const createInnerYScale = (teamName) => {
      const teamTop = yScale(teamName);
      const teamBottom = teamTop + yScale.bandwidth();

      // Create the scales
      return d3.scaleBand()
        .domain(sources)
        .range([teamTop, teamBottom])
        .padding(0.1)
        .round(true);
    };

    const xScale_InterState = d3.scaleBand()
      .domain(states)
      .range([ margin.left, width + margin.left ])
      .padding(0.0)
      .round(true);

    const xScale_State = d3.scaleLinear()
      .domain([ 0, 15 ])
      .range([ labelMargin, xScale_InterState.bandwidth() - labelMargin]);

    const scales = {
      outerYScale:yScale,
      innerYScale:null,
      outerXScale:xScale_InterState,
      innerXScale:xScale_State,
      createInnerYScale
    }

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

    // Create the chart for each 'category'
    const charts = svg.selectAll("g.stackedBars")
      .data(dataSums)
      .enter()
      .append("g")
      .attr("class", "stackedBars")
      .attr("id", d => "xAxis_" + d.cat0Key)
      .attr("width", xScale_InterState.bandwidth());

    charts.each(function(d,i){
      const chart = d3.select(this);

      const xa = chart
        .append("g")
        .call(xAxis)
        .attr("transform", "translate(" + xScale_InterState(d.cat0Key) + "," + svgHeight + ")");

      xa.selectAll("path.domain")
        .attr("opacity", "0.2");

      xa.selectAll("g.tick > line")
        .attr("opacity","0.2");

      chart.append("svg")
        .attr("class", "axisLabel")
        .attr("width", xScale_InterState.bandwidth())
        .attr("height", labelHeight)
        .attr("x", xScale_InterState(d.cat0Key) )
        .attr("y", (svgHeight + 10))
        .append("text")
        .text(d => d.cat0Key)
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .style("font-size", "14px")
        .attr("y", "50%")
        .attr("x", "50%");

      // Create a barRegion for each 'team'
      const barRegions = chart.selectAll("svg.state")
        .data(d.cat1Values)
        .enter()
        .append("svg")
        .attr("id", d => d.cat0Key)
        .attr("class", "state")
        .attr("x", xScale_InterState(d.cat0Key))
        .attr("y", margin.top);

      barRegions.each(function(team,i1){
        const teamBarRegion = d3.select(this);
        const yScale_Source = createInnerYScale(team.cat1Key);

        teamBarRegion.selectAll("svg.source_total")
          .data(team.cat2Values)
          .enter()
          .append("svg")
          .attr("class", "source_total")
          .attr("y", d1 => yScale_Source(d1.cat2Key))
          .attr("x", "0")
          .attr("width", labelMargin)
          .attr("height", yScale_Source.bandwidth())
          .append("text")
          .text(d1 => d1.total)
          .attr("x", "50%")
          .attr("y", "50%")
          .style("dominant-baseline", "central")
          .style("font-size", "10px");

        // const guideLines = teamBarRegion.selectAll("line.guideLines")
        //   .data(team.values)
        //   .enter()
        //   .append("line")
        //   .attr("x1", "0")
        //   .attr("x2", "100")
        //   .attr("y1", d1 => yScale_Source(d1.key))
        //   .attr("y2", d1 => yScale_Source(d1.key))
        //   .attr("stroke-width", 2)
        //   .attr("stroke", "black");


        // Create a bar segment for each 'severity'
        const bars = teamBarRegion.selectAll("svg.bars")
          .data(team.cat2Values)
          .enter()
          .append("svg")
          .attr("class", "bars")
          .attr("x", d1 => xScale_State(0))
          .attr("y", d1 => yScale(team.cat1Key))
          .attr("height", d1 => yScale.bandwidth())
          .attr("width", d1 => xScale_State(d1.total) - labelMargin);

        bars.each(buildCat3Bars(this,teamBarRegion,scales,config));
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
  })
  }
}
