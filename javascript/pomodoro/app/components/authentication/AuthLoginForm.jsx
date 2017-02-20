import React from 'react';
import TextInput from '../../common/TextInput';
import Button from '../../common/Button';


export default class AuthLoginForm extends React.Component {
    constructor (props) {
        super (props);
        this.state = { emailAddress: "", password: "" };
    }
    setEmailAddress = (event) => {
        event.preventDefault();
        this.setState({ emailAddress: event.target.value });
    }
    setPassword = (event) => {
        event.preventDefault();
        this.setState({ password: event.target.value });
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

                  <Button text="Login" />
                </form>
            </div>;
    }
}
