import React from 'react';
import TextInput from '../../common/TextInput';
import Button from '../../common/Button';

export default class AuthRegistrationForm extends React.Component {
    self = this
    constructor (props) {
        super (props);
        this.state = { emailAddress: "Bravo@c.com", password: "Password1!", passwordRepeat: "Password1!" };
    }
    setEmailAddress = (event) => {
        event.preventDefault();
        this.setState({ emailAddress: event.target.value });
    }
    setPassword = (event) => {
        event.preventDefault();
        this.setState({ password: event.target.value });
    }
    setPasswordRepeat = (event) => {
        event.preventDefault();
        this.setState({ passwordRepeat: event.target.value });
    }
    onRegistrationClick = (event) => {
      event.preventDefault ();
      var states = this.state;
      var data = {
        "Email": states.emailAddress,
        "Password": states.password,
        "ConfirmPassword": states.passwordRepeat
      };

      var xhttp = new XMLHttpRequest ();
      xhttp.onreadystatechange = function () {
        console.log("Response: " + xhttp.responseText);
        debugger;
      }
      xhttp.open("POST", "http://localhost:14662/api/Account/Register", true);
      xhttp.setRequestHeader("Content-type", "application/json");
      xhttp.send(JSON.stringify(data));
      debugger;
      /*
      $.ajax({
        type: 'POST',
        url: '/api/Account/Register',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data)
      }).done(function (data) {
        console.log(data);
        debugger;
      }).fail(function(error) {
        console.log("error: " + error)
      });
      debugger;
      */
    }
    render () {
        return <div className="panel panel-default">
                <div className="panel-heading">Register user</div>
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

                  <TextInput
                      name="passwordRepeat"
                      className="form-control"
                      id="passwordRepeat"
                      label="Password Again:"
                      placeholder="Enter your password again"
                      inputType="password"
                      value={this.state.passwordRepeat}
                      onChange={this.setPasswordRepeat} />


                    <Button text="Register User" onClick={this.onRegistrationClick}/>
                </form>
            </div>;
    }
}
