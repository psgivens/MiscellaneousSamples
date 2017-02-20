import React from 'react';

export default class Redirect extends React.Component {
    constructor(props) {
        super(props);
        this.handleClick = this.handleClick.bind(this);
      }

      handleClick(e) {
        e.preventDefault();

        this.context.router.push('/')
      }
    
//    componentWillMount(){
//        console.log("Mounting with properties.");
//        console.log("one: "+ this.props.params.one);
//        
//        // This is how we do a transition:
//        this.context.router.push('/')
//    }
    
    render () {
        return <div>
            <div onClick={this.handleClick}>[Click to redirect to home!]</div>
        </div>;
    }
}

Redirect.contextTypes = {
  router: React.PropTypes.object.isRequired,
};