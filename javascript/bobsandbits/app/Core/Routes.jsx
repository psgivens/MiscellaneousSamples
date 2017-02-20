import React from 'react';
import { Router, Route, hashHistory, IndexRoute } from 'react-router'

import About from '../components/About';
import App from './App';

import Home from '../components/Home';
import TestProps from '../components/TestProps';
import Redirect from '../components/Redirect';
import OpenAFile from '../components/OpenAFile';

export default class Routes extends React.Component {
  render () {
      return <Router history={hashHistory}>
          <Route path="/" component={App} >
              <IndexRoute component={Home} />
              <Route path="/about" component={About} />
              <Route path="/testprops/:one" component={TestProps} />
              <Route path="/redirect" component={Redirect} />
              <Route path="/openafile" component={OpenAFile} />
          </Route>
        </Router>
  }
}
