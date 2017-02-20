import React from 'react';

export default class TextInput extends React.Component {
    render () {
        var input = <div><input type="text"
            name={this.props.name}
            className="form-control"
            placeholder={this.props.placeholder}
            ref={this.props.name}
            value={this.props.value}
            onChange={this.props.onChange}
            type={this.props.inputType ? this.props.inputType : "text"}
            size="60" /></div>;
        return this.props.label ? <div className="form-group">
                        <label htmlFor={this.props.name}>{this.props.label}</label>
                        {input}
                    </div> : input;
    }
}
