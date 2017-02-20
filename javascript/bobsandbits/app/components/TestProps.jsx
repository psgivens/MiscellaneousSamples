import React from 'react';

export default class TestProps extends React.Component {
    componentWillMount(){
        console.log("Mounting with properties.");
        console.log("one: "+ this.props.params.one);        
        
    }
    render () {        
        var value = Number.parseInt(this.props.params.one);        
        if (isNaN(value)){
            value = "Entered a non-number";
        }
        return <div>
            Testing Properties<br />
            one: {value}
        </div>;
    }
}


