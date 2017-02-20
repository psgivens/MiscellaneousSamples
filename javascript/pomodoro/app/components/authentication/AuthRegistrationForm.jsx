import React from 'react';
import TextInput from '../../common/TextInput';
import Button from '../../common/Button';

export default class AuthRegistrationForm extends React.Component {
    constructor (props) {
        super (props);
        this.state = { emailAddress: "", password: "", passwordRepeat: "" };
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
                            value={this.state.secondNumber}
                            onChange={this.setSecondNumber} />


                        <Button text="Register User" />
                </form>
            </div>;
    }
}
