import React from 'react';
import TextInput from '../../common/TextInput';
import PomodoroStore from './PomodoroStore';
import PomodoroActions from './PomodoroActions';
import PomodoroList from './PomodoroList';


export default class PomodoroPage extends React.Component {
  constructor (params) {
    super(params);
    
    this.state = {
      currentTask: "Doit",
          timeRemaining: {
              'total': 0,
              'days': 0,
              'hours': 0,
              'minutes': 0,
              'seconds': 0
          },
          taskHistory: []
    };
  }

    componentWillMount () {
      PomodoroStore.addChangeListener(this._onChange);
    }

//Clean up when this component is unmounted
    componentWillUnmount () {
      PomodoroStore.removeChangeListener(this._onChange);
    }

    _onChange = () => {
        this.setState({
                timeRemaining: PomodoroStore.getTimeRemaining(),
                currentTask: PomodoroStore.getCurrentTask(),
                runningTask: PomodoroStore.getRunningTask(),
                taskHistory: PomodoroStore.getTaskHistory()
            });
      }

      startTask = (event) => {
              event.preventDefault();
              PomodoroActions.startTask(this.state.currentTask);
      }

      setTaskState = (event) => {
              // Does not go through the actions and stores

      		this.setState({dirty: true});
      		var field = event.target.name;
      		var value = event.target.value;
      		this.state.currentTask = value;
      		return this.setState({currentTask: this.state.currentTask});
      	}


        render () {
          return (
            <div>
              <h1>Pomodoro</h1>
                      Clock: {("0" + this.state.timeRemaining.minutes).slice(-2)}:{("0" + this.state.timeRemaining.seconds).slice(-2)}
                                       &nbsp;for {this.state.runningTask}
                      <form>
                          <TextInput
                              name="currentTask"
                              label="Current Task"
                              onChange={this.setTaskState}
                              value={this.state.currentTask} />

                              <input type="submit" value="Start" className="btn btn-default" onClick={this.startTask} />
                      </form>
                      <PomodoroList pomodoros={this.state.taskHistory} />
            </div>
          );
        }
}
