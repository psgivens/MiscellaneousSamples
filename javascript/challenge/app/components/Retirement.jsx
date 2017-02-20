import React from 'react';
import TextInput from '../common/TextInput.jsx';

export default class Retirement extends React.Component {
    constructor(props) {
        super(props);
        this.state = { age: "", retirementYear: ""};
    }

    render () {
        return <div>
            <h1>Retirment</h1>
        </div>;

        var comment = <div>
            <h1>Retirment</h1>
            <form role="form">
                <div className="form-group">
                    <label htmlFor="firstNumber">First Number:</label>
                    <TextInput
                        name="someInput"
                        className="form-control"
                        id="firstNumber"
                        placeholder="First number please"
                        value={this.state.firstNumber}
                        onChange={this.setFirstNumber} />
                </div>
                <div className="form-group">
                    <label htmlFor="secondNumber">Second Number:</label>
                    <TextInput
                        name="someInput"
                        className="form-control"
                        id="secondNumber"
                        placeholder="Second number please"
                        value={this.state.secondNumber}
                        onChange={this.setSecondNumber} />
                </div>
            </form>
            {response}
        </div>;
    }
}
