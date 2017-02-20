import React from 'react';
import Button from 'react-button';
var MyWorker = require('worker!./tasks-csvparser.js');


function logError (errorCode, errorMessage) {
  console.log (errorCode +':'+ errorMessage);
}

export default class OpenAFile extends React.Component {
  constructor (props) {
      super (props);
      this.state = { key: "value" };
  }

  logError (errorCode, errorMessage) {
    console.log (errorCode +':'+ errorMessage);
  }
  openFile (event) {

    var worker1 = new MyWorker();
    worker1.addEventListener('message', function(e) {
      var tasks = e.data;
      console.log(tasks);
    }, logError);
    worker1.postMessage("Sending this task");


    var files = document.getElementById('myFile').files;
    if (files.length === 0) {
      console.log("Please choose a file");
    } else {
      console.log("File: " + files[0].name);

      var reader = new FileReader();
      reader.onload = function(evt) {
        var contents = evt.target.result;
        var lines = contents.split('\n');
        for (var indx = 0; indx < lines.length; indx++) {
          var val = lines[indx];
          console.log("Line(" + indx + "): " + val);
        }

      }
      reader.readAsText(files[0]);
    }


/*
    var files = document.getElementById('myFile').files;
    if (files.length === 0) {
      console.log("Please choose a file");
    } else {
      console.log("File: " + files[0].name);
    }
    var reader = new FileReader();
    reader.onload = function(evt) {
      var contents = evt.target.result;
      var worker = new MyWorker();
      worker.addEventListener('message', function(e) {
        var tasks = data;
        debugger;
      }, errorLogger);
      worker.postMessage("Sending this task");
    }
    */
  }
  render () {
    return <div>
      <h1>Open A File</h1>
      <form role="form">
        <div className="form-group">
            <label htmlFor="myflile">File:</label>
            <input type="file"
                name="myFile"
                multiple
                className="form-control"
                id="myFile"
                placeholder="Choose a folder"
                />
        </div>
        <button type="button" onClick={this.openFile}>Show contents</button>
      </form>
    </div>;
  }
}
