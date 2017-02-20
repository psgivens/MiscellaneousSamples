import React from 'react';
import TextInput from '../common/TextInput.jsx';

export default class PrintQuote extends React.Component {
    constructor(props) {
        super(props);
        this.state = { quote: "", author: ""};
    }
    setQuote = (event) => {
        event.preventDefault();
        this.setState({ quote: event.target.value });
    }
    setAuthor = (event) => {
        event.preventDefault();
        this.setState({ author: event.target.value });
    }
    render () {
        var response = "";
        if (!/^\s*$/.test(this.state.author) && !/^\s*$/.test(this.state.quote))
            response = this.state.author + ' once said, \"' + this.state.quote + '"';
        return <div>
            <h1>Print Quote</h1>
            <form role="form">
                <div className="form-group">
                    <label htmlFor="author">Author:</label>
                    <TextInput
                        name="someInput"
                        className="form-control"
                        id="author"
                        placeholder="Enter the author of the quote"
                        value={this.state.author}
                        onChange={this.setAuthor} />
                </div>
                <div className="form-group">
                    <label htmlFor="quote">Quote:</label>
                    <TextInput
                        name="someInput"
                        className="form-control"
                        id="quote"
                        placeholder="Enter a quotation to express"
                        value={this.state.quote}
                        onChange={this.setQuote} />
                </div>
            </form>
            {response}
        </div>
    }
}
