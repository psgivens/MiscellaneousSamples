import React from 'react';
//import TextInput from './TextInput.jsx';
import Header from './Header.jsx';
import { Link } from 'react-router';


export default class App extends React.Component {
//    constructor (props) {
//        super (props);
//        this.state = { someValue: "" };
//    }
//
//    setValue = (event) => {
//        event.preventDefault();
//        this.setState({ someValue: event.target.value });
//    }

    render () {
        return <div>
            <Header />
            {this.props.children}
        </div>;
    }
//    render () {
//        return <div>
//            <Header />
//            <RouteHandler />
//            <TextInput
//                name="someInput"
//                placeholder="Enter a value to see it's length"
//                value={this.state.someValue}
//                onChange={this.setValue} />
//            <br />
//            "{this.state.someValue}" contains {this.state.someValue.length} characters.
//        </div>;
//    }
}
