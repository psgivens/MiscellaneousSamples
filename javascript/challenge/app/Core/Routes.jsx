import React from 'react';
import { Router, Route, hashHistory, IndexRoute } from 'react-router'

import About from '../components/About';
import App from './App';

import Home from '../components/Home';
import LengthModule from '../components/LengthModule';
import PrintQuote from '../components/PrintQuote';
import DoMath from '../components/DoMath';
import Retirement from '../components/Retirement'

export default class Routes extends React.Component {
  render () {
      return <Router history={hashHistory}>
          <Route path="/" component={App} >
              <IndexRoute component={Home} />
              <Route path="/about" component={About} />
              <Route path="/length" component={LengthModule} />
              <Route path="/printquote" component={PrintQuote} />
              <Route path="/domath" component={DoMath} />
              <Route path="/retirement" component={Retirement} />
          </Route>
        </Router>
  }
}
