"use strict";

var keyMirror = require('keyMirror');

module.exports = keyMirror({
	INITIALIZE: null,
	CREATE_AUTHOR: null,
	UPDATE_AUTHOR: null,
	DELETE_AUTHOR: null,
    TASK_STARTED: null,
    TASK_COMPETED: null,
    TASK_TIME_TICKED: null,
    CREATE_ACTION_ITEM: null
});
