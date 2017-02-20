import React from 'react';
import TextInput from '../common/TextInput.jsx';

export default class DoMath extends React.Component {
    constructor(props) {
        super(props);
        this.state = { firstNumber: "", secondNumber: ""};
    }
    setFirstNumber = (event) => {
        event.preventDefault();
        var first = Number.parseInt(event.target.value);
        if (!Number.isNaN(first))
            this.setState({ firstNumber: first });
        else if (event.target.value === "")
            this.setState({ firstNumber: "" });
    }
    setSecondNumber = (event) => {
        event.preventDefault();
        var second = Number.parseInt(event.target.value);
        if (!Number.isNaN(second))
            this.setState({ secondNumber: second });
        else if (event.target.value === "")
            this.setState({ secondNumber: "" });
    }
    render () {
        var first = this.state.firstNumber;
        var second = this.state.secondNumber;
        var response;
        if (Number.isNaN(first) || Number.isNaN(second) || first === "" || second === "")
            response = <div>Please enter a number in each field</div>;
        else{
            response = <div>
                <p>{first} + {second} = {first + second}</p>
                <p>{first} - {second} = {first - second}</p>
                <p>{first} * {second} = {first * second}</p>
                <p>{first} / {second} = {first / second}</p>
            </div>;
        }
        return <div>
            <h1>Do Math</h1>
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
