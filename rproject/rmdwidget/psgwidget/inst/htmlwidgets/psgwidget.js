HTMLWidgets.widget({

  name: 'psgwidget',

  type: 'output',

  factory: function(el, width, height) {

    // TODO: define shared variables for this instance

    return {

      renderValue: function(x) {

        // TODO: code to render the widget, e.g.
        el.innerText = "Message from psgwidget: " + x.message;
		el.style.color = 'red'
		el.style.fontWeight = 'bold'

      },

      resize: function(width, height) {

        // TODO: code to re-render the widget with a new size

      }

    };
  }
});