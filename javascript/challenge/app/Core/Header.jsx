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
                <li><NavLink to="/length">2. Length</NavLink></li>
                <li><NavLink to="/printquote">3. Print Quote</NavLink></li>
                <li><NavLink to="/domath">4. Do Math</NavLink></li>
                <li><NavLink to="/retirement">5. Retirement</NavLink></li>
              </ul>
          </div>
        </nav>
    }
}
