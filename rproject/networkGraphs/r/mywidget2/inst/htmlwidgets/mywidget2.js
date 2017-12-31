HTMLWidgets.widget({

  name: 'mywidget2',

  type: 'output',

  factory: function(el, width, height) {

    // TODO: define shared variables for this instance

    return {

      renderValue: function(x) {

        var dataset = [ 5, 10 , 15 ,20 , 25];
        d3.select(el).selectAll("p")
          .data(dataset)
          .enter()
          .append("p")
          .text("Sample"");
          //.text(x.message);
      // var svg = d3.select(el).append("svg")
      //          .attr("width", width)
      //          .attr("height", height);
      //
      //   el.innerText = x.message;

      },

      resize: function(width, height) {

        // TODO: code to re-render the widget with a new size

      }

    };
  }
});
