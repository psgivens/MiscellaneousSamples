HTMLWidgets.widget({

  name: 'psgwidget',

  type: 'output',

  factory: function(el, width, height) {

    // TODO: define shared variables for this instance
    const summarizeData = function (data, teams) {
        const teamSums = d3.nest()
          .key(function(d){ return d.state; })
          .key(function(d){ return d.team; })
          .key(function(d){ return d.severity; })
          .rollup(function(d){return d3.sum(d, function(d1){ return d1.count;});})
          .entries(data);

        const mapTeam = function(team){
          const severities = team.values;
          var offset = 0;
          severities.forEach(function(item){
            item.x0 = offset;
            offset += item.value;
            item.x1 = offset;
          });
          return {
            key:team.key,
            values:severities,
            total:severities.reduce(function(acc,item){ return acc+item.value;},0)
          };
        };

        const teamSumsR = teamSums
          .map(function(stateItem) { return Object.create({
              state:stateItem.key,
              teams:stateItem.values.map(mapTeam)
            });});

        return teamSumsR;
      };


    return {

      renderValue: function(args) {

        // TODO: code to render the widget, e.g.

        console.log("version 1");
        console.log(args);

        const incomingData = HTMLWidgets.dataframeToD3(args.data);

            // Munge the data
        const teams = Object.keys(incomingData.reduce(function(acc,d1){ acc[d1.team] = true; return acc; }, {}));
        const severities  = Object.keys(incomingData.reduce(function(acc,d1){ acc[d1.severity] = true; return acc;}, {}));
        const dataSums = summarizeData(incomingData, teams);
        //const x = y => y;

        console.log(dataSums);

    		const svg = d3.select(el)
    		  .append("svg")
    		  .attr("width","900")
    		  .attr("height","500");





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



    		d3.select(el)
    		  .selectAll("div.defects")
    		  .data(incomingData)
    		  .enter()
    		  .append("div")
    		  .attr("class", "defects")
    		  .text(function(d){ return d.severity; } );

        // Create the scales
        const yScale = d3.scaleBand()
          .domain(teams)
          .range([margin.top, svgHeight - margin.bottom])
          .padding(0.2)
          .round(true);

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
          .tickSize(0-svgHeight)
          .ticks(10);


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

          .attr("id", function(d){ return "xAxis_" + d.state; })
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
            .attr("x", xScale_InterState(d.state))
            .attr("y", svgHeight + 10)
            //.attr("transform", "translate(" + (xScale_InterState(d.state)) + "," + (svgHeight + 10)  + ")")
            .append("text")
            .text(function(d){ return d.state; })
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
            .attr("id", function(d){ return d.state; })
            .attr("class", "state")
            .attr("x", xScale_InterState(d.state))
            .attr("y", margin.top );
//            .attr("transform", function(d1){ return "translate(" + xScale_InterState(d.state) + ", " + margin.top + ")"; });


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
            .attr("x", function(d1){ return xScale_State(d1.x0); })
            .attr("y", yScale(team.key))
            .attr("class", "measurementRectangle")
            .attr("height", yScale.bandwidth())
            .attr("width", function(d1){ return xScale_State(d1.value) - labelMargin; });

          bars.append("rect")
            .style("stroke", "black")
            .style("stroke-width", "1px")
            .attr("x","0")
            .attr("y","0")
            .attr("class", function(d){ return "measurementRectangle " + d.key; })
            .attr("height", yScale.bandwidth())
            .attr("width", function(d1){ return xScale_State(d1.value) - labelMargin; });

          bars.append("text")
            .text(function(d1){ return d1.value; })
            .style("text-anchor", "middle")
            .style("dominant-baseline", "central")
            .style("fill", "black")
            .attr("y", "50%")
            .attr("x", "50%");

          });


        });













      },

      resize: function(width, height) {

        // TODO: code to re-render the widget with a new size

      }

    };
  }
});
