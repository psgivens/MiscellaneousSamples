import React from 'react';
import { Router, Route, hashHistory, IndexRoute } from 'react-router'

import About from '../components/About';
import App from './App';

import Home from '../components/Home';
import AuthenticationTest from '../components/authentication/AuthenticationTest';
import PomodoroPage from '../components/pomodoro/PomodoroPage';

export default class Routes extends React.Component {
  render () {
      return <Router history={hashHistory}>
          <Route path="/" component={App} >
              <IndexRoute component={Home} />
              <Route path="/about" component={About} />
              <Route path="/authenticate" component={AuthenticationTest} />
              <Route path="/pomodoro" component={PomodoroPage} />
          </Route>
        </Router>
  }
}
