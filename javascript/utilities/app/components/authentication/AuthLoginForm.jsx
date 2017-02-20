

import React from 'react';
import Dispatcher from '../../common/AppDispatcher';
import ActionTypes from '../../common/ActionTypes';
import AuthenticationStore from './AuthenticationStore';
import TextInput from '../../common/TextInput';
import Button from '../../common/Button';

export default class AuthLoginForm extends React.Component {
    constructor (props) {
        super (props);
        this.state = { emailAddress: "Bravo@c.com", password: "Password1!" };
    }
    setEmailAddress = (event) => {
        event.preventDefault();
        this.setState({ emailAddress: event.target.value });
    }
    setPassword = (event) => {
        event.preventDefault();
        this.setState({ password: event.target.value });
    }
    onLoginClick = (event) => {
      event.preventDefault ();

      var states = this.state;

      var details = {
        "grant_type": "password",
        "username": states.emailAddress,
        "password": states.password
      };
      var formBody = [];
      for (var property in details) {
        var encodedKey = encodeURIComponent(property);
        var encodedValue = encodeURIComponent(details[property]);
        formBody.push(encodedKey + "=" + encodedValue);
      }
      formBody = formBody.join("&");


      function getHeaders (state) {
        switch (state) {
          case 0: return "UNSENT";
          case 1: return "OPENED";
          case 2: return "HEADERS_RECEIVED";
          case 3: return "LOADING";
          case 4: return "DONE";
        }
      }

      var xhttp1 = new XMLHttpRequest ();
      console.log(getHeaders (xhttp1.readyState));
      xhttp1.onreadystatechange = function () {
        console.log(getHeaders(xhttp1.readyState));
      }

      xhttp1.open("POST", "http://localhost:14662/Token", true);
      xhttp1.setRequestHeader("Content-type", "application/x-www-form-urlencoded; charset=UTF-8");

      xhttp1.onload = function () {
        console.log("readyState: " + xhttp1.readyState);
        console.log("Response: " + xhttp1.responseText);

        debugger;
        Dispatcher.dispatch({
            actionType: ActionTypes.SESSION_AUTHENTICATED,
            authentication: JSON.parse(xhttp1.responseText)
        });
      }
      xhttp1.send(formBody);

/*
      var data = {
        "grant_type": "password",
        "username": states.emailAddress,
        "password": states.password
      };

      var xhttp = new XMLHttpRequest ();
      xhttp.onreadystatechange = function () {
        console.log("Response: " + xhttp.responseText);
        debugger;
      }
      xhttp.open("POST", "http://localhost:14662/Token", true);
      xhttp.setRequestHeader("Content-type", "application/json");
      xhttp.send(JSON.stringify(data));
      */
    }
    render () {
        return <div className="panel panel-default">
                <div className="panel-heading">User login</div>
                <form role="form">
                  <TextInput
                      name="emailAddress"
                      className="form-control"
                      id="emailAddress"
                      label="Email Address:"
                      placeholder="Enter your email address"
                      value={this.state.emailAddress}
                      onChange={this.setEmailAddress} />

                  <TextInput
                      name="password"
                      className="form-control"
                      id="password"
                      label="Password:"
                      placeholder="Enter your password"
                      inputType="password"
                      value={this.state.password}
                      onChange={this.setPassword} />

                      <Button text="Login" onClick={this.onLoginClick} />
                </form>
            </div>;
    }
}
