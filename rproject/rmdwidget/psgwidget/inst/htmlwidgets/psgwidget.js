HTMLWidgets.widget({

  name: 'psgwidget',

  type: 'output',

  factory: function(el, width, height) {

    // TODO: define shared variables for this instance

    return {

      renderValue: function(args) {

        // TODO: code to render the widget, e.g.
        
        console.log("version 1");
        console.log(args);
    		  
    		const svg = d3.select(el)
    		  .append("svg")
    		  .attr("width","500")
    		  .attr("height","500")
    		  .append("rect")
    		  .attr("width", "100")
    		  .attr("height", "100");
    		  
    		d3.select(el)
    		  .selectAll("div.defects")
    		  .data(HTMLWidgets.dataframeToD3(args.data))
    		  .enter()
    		  .append("div")
    		  .attr("class", "defects")
    		  .text(d=>d.severity);

//    		el.innerText = "Message from psgwidget: " ;
//    		el.style.color = 'red';
//    		el.style.fontWeight = 'bold';

      },

      resize: function(width, height) {

        // TODO: code to re-render the widget with a new size

      }

    };
  }
});