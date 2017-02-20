"use strict";

import React from 'react';
import { Router, Link } from 'react-router';
import NavLink from '../common/NavLink';

export default class Header extends React.Component {
    render () {
        return <nav className="navbar navbar-default">
          <div className="container-fluid">
              <Link to="app" className="navbar-brand">

              </Link>
              <ul className="nav navbar-nav">
                <li><NavLink to="/" onlyActiveOnIndex>Home</NavLink></li>
                <li><NavLink to="/about">About</NavLink></li>
                <li><NavLink to="/redirect">Redirect</NavLink></li>
                <li><NavLink to="/testprops/someValue">Test Props</NavLink></li>
                <li><NavLink to="/openafile">Open a file</NavLink></li>
              </ul>
          </div>
        </nav>
    }
}
