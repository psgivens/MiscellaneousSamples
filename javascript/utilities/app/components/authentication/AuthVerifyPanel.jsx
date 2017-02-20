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
    onLoginClick = (event) => {
      event.preventDefault ();
      debugger;

      function getHeaders (state) {
        switch (state) {
          case 0: return "UNSENT";
          case 1: return "OPENED";
          case 2: return "HEADERS_RECEIVED";
          case 3: return "LOADING";
          case 4: return "DONE";
        }
      }

      var xhttp1 = new XMLHttpRequest ();
      console.log(getHeaders (xhttp1.readyState));
      xhttp1.onreadystatechange = function () {
        console.log(getHeaders(xhttp1.readyState));
      }

      xhttp1.open("GET", "http://localhost:14662/api/v1/home", true);
      xhttp1.setRequestHeader("Content-type", "application/x-www-form-urlencoded; charset=UTF-8");
      xhttp1.setRequestHeader("Authorization", "Bearer")
      xhttp1.onload = function () {
        console.log("readyState: " + xhttp1.readyState);
        console.log("Response: " + xhttp1.responseText);
      }
      xhttp1.send(formBody);

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

                          <Button text="Post" onClick={this.onLoginClick} />
                </form>
            </div>;
    }
}
