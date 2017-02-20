import React from 'react';
import TextInput from '../common/TextInput.jsx';

export default class LengthModule extends React.Component {
    constructor (props) {
        super (props);
        this.state = { someValue: "" };
    }

    setValue = (event) => {
        event.preventDefault();
        this.setState({ someValue: event.target.value });
    }

    render () {
        return <div>
            <TextInput
                name="someInput"
                placeholder="Enter a value to see it's length"
                value={this.state.someValue}
                onChange={this.setValue} />
            <br />
            "{this.state.someValue}" contains {this.state.someValue.length} characters.
        </div>;
    }
}
