var gauge = function(container, configuration) {
  const colors = ["#16A085", "#76D7C4", "#F7DC6F", "#F1C40F", "#A93226"];
	var that = {};
	var config = {
		size						: 110,
		clipWidth					: 200,
		clipHeight					: 110,
		ringInset					: 20,
		ringWidth					: 20,

		pointerWidth				: 10,
		pointerTailLength			: 5,
		pointerHeadLengthPercent	: 0.9,

		minValue					: 0,
		maxValue					: 10,

		minAngle					: -90,
		maxAngle					: 90,

		transitionMs				: 750,

		majorTicks					: 5,
		labelFormat					: d3.format('d'),
		labelInset					: 10,

    arcColorFn          : (d,i) => colors[i] //#F9EBEA
		//arcColorFn					: (d,i) => d3.interpolateHsl(d3.rgb('#F9E79F'), d3.rgb('#641E16'))(d*i)
	};
	var range = undefined;
	var r = undefined;
	var pointerHeadLength = undefined;
	var value = 0;

	var svg = undefined;
	var arc = undefined;
	var scale = undefined;
	var ticks = undefined;
	var tickData = undefined;
	var pointer = undefined;

	var donut = d3.pie();

	function deg2rad(deg) {
		return deg * Math.PI / 180;
	}

	function newAngle(d) {
		var ratio = scale(d);
		var newAngle = config.minAngle + (ratio * range);
		return newAngle;
	}

	function configure(configuration) {
		var prop = undefined;
		for ( prop in configuration ) {
			config[prop] = configuration[prop];
		}

		range = config.maxAngle - config.minAngle;
		r = config.size / 2;
		pointerHeadLength = Math.round(r * config.pointerHeadLengthPercent);

		// a linear scale that maps domain values to a percent from 0..1
		scale = d3.scaleLinear()
			.domain([config.minValue, config.maxValue])
      .range([0,1]);

    // ticks = scale.ticks(config.majorTicks);
    ticks = scale.ticks(0);
		tickData = d3.range(config.majorTicks).map(function() {return 1/config.majorTicks;});

		arc = d3.arc()
			.innerRadius(r - config.ringWidth - config.ringInset)
			.outerRadius(r - config.ringInset)
			.startAngle(function(d, i) {
				var ratio = d * i;
				return deg2rad(config.minAngle + (ratio * range));
			})
			.endAngle(function(d, i) {
				var ratio = d * (i+1);
				return deg2rad(config.minAngle + (ratio * range));
			});
	}
	that.configure = configure;

	function centerTranslation() {
		return 'translate('+r +','+ r +')';
	}

	function isRendered() {
		return (svg !== undefined);
	}
	that.isRendered = isRendered;

	function render(newValue) {
		svg = d3.select(container)
			.append('svg:svg')
				.attr('class', 'gauge')
				.attr('width', config.clipWidth)
				.attr('height', config.clipHeight);

		var centerTx = centerTranslation();

		var arcs = svg.append('g')
				.attr('class', 'arc')
				.attr('transform', centerTx);

		arcs.selectAll('path')
				.data(tickData)
			  .enter()
        .append('path')
				.attr('fill', function(d, i) {
					return config.arcColorFn(d,i);
				})
				.attr('d', arc);

		var lg = svg.append('g')
				.attr('class', 'label')
				.attr('transform', centerTx);
		lg.selectAll('text')
				.data(ticks)
			.enter().append('text')
				.attr('transform', function(d) {
					var ratio = scale(d);
					var newAngle = config.minAngle + (ratio * range);
					return 'rotate(' +newAngle +') translate(0,' +(config.labelInset - r) +')';
				})
				.text(config.labelFormat);

		var lineData = [ [config.pointerWidth / 2, 0],
						[0, -pointerHeadLength],
						[-(config.pointerWidth / 2), 0],
						[0, config.pointerTailLength],
						[config.pointerWidth / 2, 0] ];
		var pointerLine = d3.line().curve(d3.curveLinear)
		var pg = svg.append('g').data([lineData])
				.attr('class', 'pointer')
				.attr('transform', centerTx);

		pointer = pg.append('path')
			.attr('d', pointerLine/*function(d) { return pointerLine(d) +'Z';}*/ )
			.attr('transform', 'rotate(' +config.minAngle +')');

		update(newValue === undefined ? 0 : newValue);
	}
	that.render = render;
	function update(newValue, newConfiguration) {
		if ( newConfiguration  !== undefined) {
			configure(newConfiguration);
		}
		var ratio = scale(newValue);
		var newAngle = config.minAngle + (ratio * range);
		pointer.transition()
			.duration(config.transitionMs)
			.ease(d3.easeElastic)
			.attr('transform', 'rotate(' +newAngle +')');
	}
	that.update = update;

	configure(configuration);

	return that;
};

function startGauge(gaugeId, pointCapacity) {
	const powerGauge = gauge('#' + gaugeId, {
		size: 200,
		clipWidth: 200,
		clipHeight: 120,
		ringWidth: 60,
		maxValue: pointCapacity,
//    majorTicks: pointCapacity/2,
		transitionMs: 4000,
	});
	powerGauge.render();
  //
	// function updateReadings() {
	// 	// just pump in random data here...
	// 	powerGauge.update(Math.random() * 10);
	// }
  //
	// // every few seconds update reading values
	// updateReadings();
	// setInterval(function() {
	// 	updateReadings();
	// }, 5 * 1000);
  return powerGauge;
}

// if ( !window.isLoaded ) {
// 	window.addEventListener("load", function() {
// 		onDocumentReady();
// 	}, false);
// } else {
// 	onDocumentReady();
// }
