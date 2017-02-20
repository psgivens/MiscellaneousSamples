import React from 'react';
import TextInput from '../../common/TextInput.jsx';
import AuthRegistrationForm from './AuthRegistrationForm';
import AuthLoginForm from './AuthLoginForm';
import AuthVerifyPanel from './AuthVerifyPanel';

export default class AuthenticationTest extends React.Component {
    constructor (props) {
        super (props);
        this.state = { someValue: "" };
    }
    render () {
        return <div>
            <h1>Authentication Test</h1>
            <div className="col-lg-3"><AuthRegistrationForm /></div>
            <div className="col-lg-2"><AuthLoginForm /></div>
            <div className="col-lg-3"><AuthVerifyPanel /></div>
        </div>;
    }
}
