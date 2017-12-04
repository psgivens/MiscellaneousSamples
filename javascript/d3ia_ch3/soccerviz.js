function createSoccerViz() {
  d3.csv("worldcup.csv", function(data) {
    overallTeamViz(data);
  })

  function overallTeamViz(incomingData) {

    const teamG = d3.select("svg")
      // create graphics element called teamsG to serve as a container
      .append("g")
      .attr("id", "teamsG")
      .attr("transform", "translate(50,300)")
      .selectAll("g")
      .data(incomingData)
      .enter()
      .append("g")
      .attr("class", "overallG")
      .attr("transform",
         function (d,i) {return "translate(" + (i * 50) + ", 0)"}
        );

      teamG
        .append("circle")
        // Style the circle
        .style("fill", "pink")
        .style("stroke", "black")
        .style("stroke-width", "1px")
        // Start with nothing and transition to 40
        .attr("r", 0)
        .transition()
        .delay(function(d,i) {return i * 100})
        .duration(500)
        .attr("r", 40)
        // Transition back to 20
        .transition()
        .duration(500)
        .attr("r", 20);

    teamG
      .append("text")
      .style("text-anchor", "middle")
      .attr("y", 30)
      .style("font-size", "10px")
      .text(function(d, i) {return d.team + " " + i;});
    const dataKeys = d3.keys(incomingData[0]).filter(function(el) {
       return el != "team" && el != "region";
    });

    teamG
      .on("mouseover", highlightRegion);

    function highlightRegion(d) {
         d3.selectAll("g.overallG")
           .select("circle")
           .style("fill", function(p) {
             return p.region == d.region ? "red" : "gray";
           });

    };

    d3.select("#controls")
      .selectAll("button.teams")
      .data(dataKeys)
      .enter()
      .append("button")
      .classed("team", true)
      .on("click", buttonClick)
      .html(function(d) {return d;});

    function buttonClick(datapoint) {
      const maxValue = d3.max(incomingData, function(d) {
        return parseFloat(d[datapoint]);
      });

      const radiusScale = d3.scaleLinear()
        .domain([ 0, maxValue ])
        .range([ 2, 20 ]);

      d3.selectAll("g.overallG")
        .select("circle")
        .transition()
        .duration(1000)
        .attr("r", function(p) {
              return radiusScale(p[datapoint]);
        });

    };
  }
}
