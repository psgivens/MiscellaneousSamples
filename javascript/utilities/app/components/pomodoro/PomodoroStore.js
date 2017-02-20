"use strict";

var Dispatcher = require('../../common/AppDispatcher');
var ActionTypes = require('../../common/ActionTypes');
var EventEmitter = require('events').EventEmitter;
var assign = require('object-assign');
var _ = require('lodash');
var CHANGE_EVENT = 'change';

var _timeRemaining;
var _currentTask;
var _taskHistory = [];
var _runningTask;

var PomodoroStore = assign({}, EventEmitter.prototype, {
	addChangeListener: function(callback) {
		this.on(CHANGE_EVENT, callback);
	},

	removeChangeListener: function(callback) {
		this.removeListener(CHANGE_EVENT, callback);
	},

	emitChange: function() {
		this.emit(CHANGE_EVENT);
	},

    getTimeRemaining: function() { return _timeRemaining; },
    getCurrentTask: function() { return _currentTask; },
    getRunningTask: function() { return _runningTask; },
    getTaskHistory: function() { return _taskHistory; }
});

Dispatcher.register(function(action) {

	switch(action.actionType) {
        case ActionTypes.TASK_STARTED:
            _timeRemaining = action.timeRemaining;
            _runningTask = action.taskTitle;
            _currentTask = "";
            _taskHistory.unshift({
                "Id": action.taskId,
                "Title": action.taskTitle,
                "Elapsed": action.timeElapsed,
                "StartTime": action.startTime
            });
            PomodoroStore.emitChange();
            break;

        case ActionTypes.TASK_TIME_TICKED:
            _timeRemaining = action.timeRemaining;
            _taskHistory[0].Elapsed = action.timeElapsed;
            //_taskHistory.find(function(task){return task.Id === action.taskId; }).Elapsed = action.timeElapsed;
            PomodoroStore.emitChange();
            break;

        case ActionTypes.TASK_COMPLETED:
            _timeRemaining = action.timeRemaining;
            _currentTask = "";
            _runningTask = "";
            _taskHistory[0].Elapsed = action.timeElapsed;
            //_taskHistory.find(function(task){return task.Id === action.taskId; }).Elapsed = action.timeElapsed;
            PomodoroStore.emitChange();
            break;
		default:
			// no op
	}
});

module.exports = PomodoroStore;
