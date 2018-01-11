function createBarCharts() {
  d3.csv("defects.csv", function(data) {
    createVisualization(data);
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
        .attr("x", d1 => scales.innerXScale(d1.x0))
        .attr("y", d1 => scales.innerYScale(source.cat2Key))
        .attr("class", "measurementRectangle")
        .attr("height", d1 => scales.innerYScale.bandwidth())
        .attr("width", d1 => scales.innerXScale(d1.value) - config.labelMargin);

      sourceBars.append("rect")
        .style("stroke", "black")
        .style("stroke-width", "1px")
        .attr("x","0")
        .attr("y","0")
        .attr("class", d => "measurementRectangle " + d.cat3Key)
        .attr("height", d1 => scales.outerYScale.bandwidth())
        .attr("width", d1 => scales.innerXScale(d1.value) - config.labelMargin);

      sourceBars.append("text")
        .text(d1 => d1.value)
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .attr("y", "50%")
        .attr("x", "50%");
      };
    };

  function buildCat2Bars(_this, scales, config) {
    return function(team,i1){
      const teamBarRegion = d3.select(this);
      const cat1Scales = Object.assign({innerYScale:scales.createInnerYScale(team.cat1Key)}, scales);

      teamBarRegion.selectAll("svg.source_total")
        .data(team.cat2Values)
        .enter()
        .append("svg")
        .attr("class", "source_total")
        .attr("y", d1 => cat1Scales.innerYScale(d1.cat2Key))
        .attr("x", "0")
        .attr("width", config.labelMargin)
        .attr("height", cat1Scales.innerYScale.bandwidth())
        .append("text")
        .text(d1 => d1.total)
        .attr("x", "50%")
        .attr("y", "50%")
        .style("dominant-baseline", "central")
        .style("font-size", "10px");

      // Create a bar segment for each 'severity'
      const bars = teamBarRegion.selectAll("svg.bars")
        .data(team.cat2Values)
        .enter()
        .append("svg")
        .attr("class", "bars")
        .attr("x", d1 => cat1Scales.innerXScale(0))
        .attr("y", d1 => cat1Scales.outerYScale(team.cat1Key))
        .attr("height", d1 => cat1Scales.outerYScale.bandwidth())
        .attr("width", d1 => cat1Scales.innerXScale(d1.total) - config.labelMargin);

      bars.each(buildCat3Bars(this,teamBarRegion,cat1Scales,config));
    };
  }

  function buildCat0Chart(_this, scales, config, xAxis) {
    return function(d,i){
      const chart = d3.select(this);

      /* Create the xAxis for the chart
      ************************/
      const axis = chart
        .append("g")
        .call(xAxis)
        .attr("transform", "translate(" + scales.outerXScale(d.cat0Key) + "," + config.svgHeight + ")");

      axis.selectAll("path.domain")
        .attr("opacity", "0.2");

      axis.selectAll("g.tick > line")
        .attr("opacity","0.2");

      /* Label the chart
      ************************/
      chart.append("svg")
        .attr("class", "axisLabel")
        .attr("width", scales.outerXScale.bandwidth())
        .attr("height", config.labelHeight)
        .attr("x", scales.outerXScale(d.cat0Key) )
        .attr("y", (config.svgHeight + 10))
        .append("text")
        .text(d => d.cat0Key)
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .style("font-size", "14px")
        .attr("y", "50%")
        .attr("x", "50%");

      /* Create the data sections of the chart
      ************************/
      const barRegions = chart.selectAll("svg.state")
        .data(d.cat1Values)
        .enter()
        .append("svg")
        .attr("id", d => d.cat0Key)
        .attr("class", "state")
        .attr("x", scales.outerXScale(d.cat0Key))
        .attr("y", config.margin.top);

      barRegions.each(buildCat2Bars(this, scales, config));

      /* Wire the chart up for interaction.
      ************************/
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
    };
  }

  function createConfig(){
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
    return {svgHeight,svgWidth,margin,width,height,catSize,labelOffset,
                    tickSize,labelMargin,labelHeight};
  }

  function extractMeta(incomingData){
    const cat0Values = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.state), new Set ()));
    const cat1Values = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.team), new Set ()));
    const cat2Values  = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.source), new Set ()));
    const cat3Values  = Array.from(incomingData.reduce((acc,d1) => acc.add(d1.severity), new Set ()));
    return {cat0Values,cat1Values,cat2Values,cat3Values,};
  }

  function buildScales(config, meta) {

    // Create the scales
    const outerXScale = d3.scaleBand()
      .domain(meta.cat0Values)
      .range([config.margin.left, config.width + config.margin.left ])
      .padding(0.0)
      .round(true);

    const outerYScale = d3.scaleBand()
      .domain(meta.cat1Values)
      .range([config.margin.top, config.svgHeight - config.margin.bottom])
      .padding(0.00)
      .round(true);

    const createInnerYScale = (cat1Key) => {
      const top = outerYScale(cat1Key);
      const bottom = top + outerYScale.bandwidth();

      // Create the scales
      return d3.scaleBand()
        .domain(meta.cat2Values)
        .range([top, bottom])
        .padding(0.1)
        .round(true);
    };

    const innerXScale = d3.scaleLinear()
      .domain([ 0, 15 ])
      .range([config.labelMargin, outerXScale.bandwidth() - config.labelMargin]);

    return {
      outerYScale:outerYScale,
      outerXScale:outerXScale,
      innerXScale:innerXScale,
      createInnerYScale:createInnerYScale
    };
  }

  function createVisualization(incomingData) {

    /* Configure the graph with sizes.
    ************************/
    const config = createConfig();

    /* Get the categories
    ************************/
    const meta = extractMeta(incomingData);

    /* Munge the data
    ************************/
    const dataSums = formatData(incomingData, ["state","team","source","severity"]);

    /* Build the scales
    ************************/
    const scales = buildScales(config, meta);

    // Define the axis functions
    const yAxis = d3.axisLeft()
      .scale(scales.outerYScale)
      .tickSize(0)
      .ticks(4);

    const xAxis = d3.axisBottom()
      .scale(scales.innerXScale)
      .tickSize(0-config.svgHeight + 20)
      .ticks(10);


    /* Start with globals for the graph.
    ************************/
    const svg = d3.selectAll("svg");

    svg.append("g")
      .attr("id","yAxisG")
      .call(yAxis)
      .attr("transform","translate(" + config.margin.left + "," + config.margin.top + ")")
      .selectAll("path.domain")
      .attr("display", "none");

    let cat1Sections = svg.selectAll("svg.cat2LabelSections")
      .data(meta.cat1Values)
      .enter()
      .append("svg")
      .attr("class","cat2LabelSections")
      .attr("transform",d1 => "translate(" + (config.margin.left + config.width) + "," + (scales.outerYScale(d1) + config.margin.top) + ")")
      .attr("height", scales.outerYScale.bandwidth() - 1)
      .attr("width", config.margin.right);
      // .append("rect")
      // .attr("width",100)
      // .attr("height",100)
      // .style("fill","black")

    cat1Sections.each(function(data,iter){
      const cat1Section = d3.select(this);
      cat1Section
        .append("text")
        .text(d1 => "d1.value")
        .style("text-anchor", "middle")
        .style("dominant-baseline", "central")
        .style("fill", "black")
        .attr("y", "50%")
        .attr("x", "50%");
    });

    /* Create the chart for each 'category'
    ************************/
    const charts = svg.selectAll("g.stackedBars")
      .data(dataSums)
      .enter()
      .append("g")
      .attr("class", "stackedBars")
      .attr("id", d => "xAxis_" + d.cat0Key)
      .attr("width", scales.outerXScale.bandwidth());

    charts.each(buildCat0Chart(this, scales, config, xAxis));
  }
}
