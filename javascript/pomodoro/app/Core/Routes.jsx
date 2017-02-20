import React from 'react';
import { Router, Route, hashHistory, IndexRoute } from 'react-router'

import LengthModule from '../components/LengthModule';
import About from '../components/About';
import App from './App';

import Home from '../components/Home';
import TestProps from '../components/TestProps';
import Redirect from '../components/Redirect';
import PrintQuote from '../components/PrintQuote';
import DoMath from '../components/DoMath';
import Retirement from '../components/Retirement'
import AuthenticationTest from '../components/authentication/AuthenticationTest';
import PomodoroPage from '../components/pomodoro/PomodoroPage';

export default class Routes extends React.Component {
  render () {
      return <Router history={hashHistory}>
          <Route path="/" component={App} >
              <IndexRoute component={Home} />
              <Route path="/about" component={About} />
              <Route path="/length" component={LengthModule} />
              <Route path="/testprops/:one" component={TestProps} />
              <Route path="/redirect" component={Redirect} />
              <Route path="/printquote" component={PrintQuote} />
              <Route path="/domath" component={DoMath} />
              <Route path="/retirement" component={Retirement} />
              <Route path="/authenticate" component={AuthenticationTest} />
              <Route path="/pomodoro" component={PomodoroPage} />
          </Route>
        </Router>

  }
}
