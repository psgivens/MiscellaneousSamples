import React from 'react';
import TextInput from '../../common/TextInput';
import Button from '../../common/Button';

export default class AuthVerifyPanel extends React.Component {
    constructor (props) {
        super (props);
        this.state = { valueToPost: "" };
    }
    setValueToPost = (event) => {
        event.preventDefault();
        this.setState({ valueToPost: event.target.value });
    }
    render () {
        return <div className="panel panel-default">
                <div className="panel-heading">Post Action</div>
                <form role="form">
                        <TextInput
                            name="valueToPost"
                            className="form-control"
                            id="valueToPost"
                            label="Value to Post:"
                            placeholder="Enter a value to post"
                            value={this.state.valueToPost}
                            onChange={this.setValueToPost} />

                          <Button text="Post" />
                </form>
            </div>;
    }
}
