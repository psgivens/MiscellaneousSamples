"use strict";

var React = require('react');
var Router = require('react-router');
var Link = Router.Link;
var AuthorActions = require('./PomodoroActions');
//var toastr = require('toastr');

var PomodoroList = React.createClass({
	propTypes: {
		pomodoros: React.PropTypes.array.isRequired
	},

//	deleteAuthor: function(id, event) {
//		event.preventDefault();
//		AuthorActions.deleteAuthor(id);
//		toastr.success('Author Deleted');
//	},

	render: function() {
		var createRow = function(task) {
            var elapsed = task.Elapsed;
            var options = {
                //weekday: "long", year: "numeric", month: "short",
                //day: "numeric",
                hour: "2-digit", minute: "2-digit"
            };
            var startTime = task.StartTime.toLocaleTimeString("en-us", options);

			return (
				<tr key={task.Id}>
					<td>{task.Title}</td>
                    <td>{startTime}</td>
					<td>{("0" + elapsed.minutes).slice(-2)}:{("0" + elapsed.seconds).slice(-2)}</td>
				</tr>
			);
		};

		return (
			<div>
				<table className="table">
					<thead>
						<th>Task</th>
						<th>Start</th>
                        <th>Elapsed</th>
					</thead>
					<tbody>
						{this.props.pomodoros.map(createRow, this)}
					</tbody>
				</table>
			</div>
		);
	}
});

module.exports = PomodoroList;
