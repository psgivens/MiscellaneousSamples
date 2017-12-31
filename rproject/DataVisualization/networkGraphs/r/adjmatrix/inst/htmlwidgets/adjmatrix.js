HTMLWidgets.widget({

  name: 'adjmatrix',

  type: 'output',

  factory: function(el, width, height) {

    // TODO: define shared variables for this instance

    return {

      renderValue: function(input) {

        // Convert the links dataframe to a list of objects.
        var links = [];
        for(var  i=0; i<input.data.links.Source.length; i++){
          links.push({
            source:input.data.links.Source[i],
            target:input.data.links.Target[i],
            value:input.data.links.Value[i]}
          );
        }

        // Convert the nodes dataframe to a list of objects.
        var nodes = [];
        for (var i=0; i<input.data.nodes.id.length; i++){
          nodes.push({
            group:(input.data.nodes.id[i]%3),
            name:input.data.nodes.name[i],
            color:input.data.nodes.color[i]
          });
        }

        var margin = {top: 80, right: 0, bottom: 10, left: 80};

        var x = d3.scaleOrdinal([0, width]),
            z = d3.scaleLinear().domain([0, 4]),
            c = d3.schemeCategory10;//.domain(d3.range(10));

        var _scaleband = d3.scaleBand().rangeRound([0, width]).domain(nodes);

        var svg = d3.select(el)
            .append("svg")
            .attr("fill", "white")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .style("margin-left", -margin.left + "px")
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

          // var miserables = dataset;
          //d3.json("irenesdata.json", function(miserables) {
          var matrix = [],
              n = nodes.length;

          // Compute index per node.
          nodes.forEach(function(node, i) {
            node.index = i;
            node.count = 0;
            matrix[i] = d3.range(n).map(function(j) { return {x: j, y: i, z: 0, r: 0}; });
          });

          // Convert links to matrix; count character occurrences.
          links.forEach(function(link) {
        	if (matrix[link.source][link.target].r === 1) {
        		matrix[link.source][link.target].r = 2;
        		matrix[link.target][link.source].r = 2;
        	} else {
        		matrix[link.target][link.source].r = 1;
        	}
            matrix[link.source][link.target].z += link.value;

            //matrix[link.target][link.source].z += link.value;

            //matrix[link.source][link.source].z += link.value;
            //matrix[link.target][link.target].z += link.value;

            nodes[link.source].count = 1; //+= link.value;
            //nodes[link.target].count += link.value;
          });

          // Precompute the orders.
          var orders = {
            name: d3.range(n).sort(function(a, b) { return d3.ascending(nodes[a].name, nodes[b].name); }),
            count: d3.range(n).sort(function(a, b) { return nodes[b].count - nodes[a].count; }),
            group: d3.range(n).sort(function(a, b) { return nodes[b].group - nodes[a].group; })
          };

          // The default sort order.
          x.domain(orders.name);
          _scaleband.domain(orders.name);

          svg.append("rect")
              .attr("class", "background")
              .attr("width", width)
              .attr("height", height)
              .attr("background", "white");

          var row = svg.selectAll(".row")
              .data(matrix)
            .enter().append("g")
              .attr("class", "row")
              .attr("transform", function(d, i) { return "translate(0," + _scaleband(i) + ")"; })
              .each(row);

          row.append("line")
              .attr("x2", width);

          row.append("text")
              .attr("fill", "black")
              .attr("x", -6)
              .attr("y", _scaleband.bandwidth() / 2)
              .attr("dy", ".32em")
              .attr("text-anchor", "end")
              .text(function(d, i) { return nodes[i].name; });

          var column = svg.selectAll(".column")
              .data(matrix)
            .enter().append("g")
              .attr("class", "column")
              .attr("transform", function(d, i) { return "translate(" + _scaleband(i) + ")rotate(-90)"; });

          column.append("line")
              .attr("x1", -width);

          column.append("text")
              .attr("fill", "black")
              .attr("x", 6)
              .attr("y", _scaleband.bandwidth() / 2)
              .attr("dy", ".32em")
              .attr("text-anchor", "start")
              .text(function(d, i) { return nodes[i].name; });

          function row(row) {
            var cell = d3.select(this).selectAll(".cell")
                .data(row.filter(function(d) { return d.z; }))
                .enter().append("rect")
                .attr("class", "cell")
                .attr("x", function(d) { return _scaleband(d.x); })
                .attr("width", 30)
                .attr("height", _scaleband.bandwidth())
                .style("fill-opacity", "0.8") // function(d) { return z(d.z); })
                .style("fill", function(d) { return matrix[d.x][d.y].r ===2 ? "#33FF33" :"#3333FF"; })
                .on("mouseover", mouseover)
                .on("mouseout", mouseout);
          }

          function mouseover(p) {
            d3.selectAll(".row text").classed("active", function(d, i) { return i == p.y; });
            d3.selectAll(".column text").classed("active", function(d, i) { return i == p.x; });
          }

          function mouseout() {
            d3.selectAll("text").classed("active", false);
          }

          // d3.select("#order").on("change", function() {
          //   clearTimeout(timeout);
          //   order(this.value);
          // });

          function order(value) {
            x.domain(orders[value]);
        	_scaleband.domain(orders.name);
            var t = svg.transition().duration(2500);

            t.selectAll(".row")
                .delay(function(d, i) { return _scaleband(i) * 4; })
                .attr("transform", function(d, i) { return "translate(0," + _scaleband(i) + ")"; })
              .selectAll(".cell")
                .delay(function(d) { return _scaleband(d.x) * 4; })
                .attr("x", function(d) { return _scaleband(d.x); });

            t.selectAll(".column")
                .delay(function(d, i) { return _scaleband(i) * 4; })
                .attr("transform", function(d, i) { return "translate(" + _scaleband(i) + ")rotate(-90)"; });
          }
      },

      resize: function(width, height) {
        // TODO: code to re-render the widget with a new size

      }
    };
  }
});
