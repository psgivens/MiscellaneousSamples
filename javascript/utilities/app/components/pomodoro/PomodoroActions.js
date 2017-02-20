"use strict";

var Dispatcher = require('../../common/AppDispatcher');
var ActionTypes = require('../../common/ActionTypes');
var CrankSound = require('../../sounds/crank.wav');
var BellSound = require('../../sounds/Zen Buddhist Temple Bell-SoundBible.com-331362457.wav');

function _getTimeRemaining(endtime){
    var timeDiff = endtime - Date.parse(new Date());
    var seconds = Math.floor( (timeDiff / 1000) % 60 );
    var minutes = Math.floor( (timeDiff / 1000 / 60) % 60 );
    var hours = Math.floor( (timeDiff / (1000 * 60 * 60)) % 24 );
    var days = Math.floor( timeDiff / (1000 * 60 * 60 * 24) );
    return {
        'total': timeDiff,
        'days': days,
        'hours': hours,
        'minutes': minutes,
        'seconds': seconds
  };
}

function _getTimeElapsed(startTime){
    var timeDiff = Date.parse(new Date()) - startTime;
    var seconds = Math.floor( (timeDiff / 1000) % 60 );
    var minutes = Math.floor( (timeDiff / 1000 / 60) % 60 );
    var hours = Math.floor( (timeDiff / (1000 * 60 * 60)) % 24 );
    var days = Math.floor( timeDiff / (1000 * 60 * 60 * 24) );
    return {
        'total': timeDiff,
        'days': days,
        'hours': hours,
        'minutes': minutes,
        'seconds': seconds
  };
}

function _playSound(path) {
  var audioElement = document.createElement('audio');
  audioElement.setAttribute('src', path);
  audioElement.play();
}

var currentTaskId = 0;

var PomodoroActions = {

    startTask: function(taskTitle) {
        // TODO: Move this to an API.
        var expire = new Date(new Date().getTime() + 5 * 1000);
        var startTime = new Date();
        var timeinterval = setInterval(function(){
            var timeRemaining = _getTimeRemaining(expire);

            if(timeRemaining.total <= 0){
                clearInterval(timeinterval);
                _playSound(BellSound);
                Dispatcher.dispatch({
                    actionType: ActionTypes.TASK_COMPLETED,
                    taskTitle: taskTitle,
                    timeRemaining: {
                        'total': 0,
                        'days': 0,
                        'hours': 0,
                        'minutes': 0,
                        'seconds': 0
                    },
                    timeElapsed: _getTimeElapsed(startTime),
                    taskId: currentTaskId
                });
            }
            else{
                Dispatcher.dispatch({
                    actionType: ActionTypes.TASK_TIME_TICKED,
                    taskTitle: taskTitle,
                    timeRemaining: timeRemaining,
                    timeElapsed: _getTimeElapsed(startTime),
                    taskId: currentTaskId
                });
            }

        }, 1000);

        _playSound(CrankSound);
        Dispatcher.dispatch({
            actionType: ActionTypes.TASK_STARTED,
            taskTitle: taskTitle,
            timeRemaining: _getTimeRemaining(expire),
            taskId: ++currentTaskId,
            timeElapsed: {
                'total': 0,
                'days': 0,
                'hours': 0,
                'minutes': 0,
                'seconds': 0
            },
            startTime: new Date()
        });
    }
};

module.exports = PomodoroActions;
