"use strict";

var Dispatcher = require('../../common/AppDispatcher');
var ActionTypes = require('../../common/ActionTypes');
var EventEmitter = require('events').EventEmitter;
var assign = require('object-assign');
var _ = require('lodash');
var CHANGE_EVENT = 'change';

var _accessToken;
var _tokentType;
var _userName;
var _issuedDate;
var _expiresDate;

var AuthenticationStore = assign({}, EventEmitter.prototype, {
	addChangeListener: function(callback) {
		this.on(CHANGE_EVENT, callback);
	},

	removeChangeListener: function(callback) {
		this.removeListener(CHANGE_EVENT, callback);
	},

	emitChange: function() {
		this.emit(CHANGE_EVENT);
	},

  getToken: function() { return _accessToken; }
});

Dispatcher.register(function(action) {
	debugger;
	switch(action.actionType) {
        case ActionTypes.SESSION_AUTHENTICATED:
            debugger;
						_accessToken = action.authentication.access_token;
            AuthenticationStore.emitChange();
            break;

		default:
			// no op
	}
});

module.exports = AuthenticationStore;
