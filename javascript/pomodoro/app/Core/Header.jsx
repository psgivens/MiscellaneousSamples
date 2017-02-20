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
                <li><NavLink to="/redirect">Redirect</NavLink></li>
                <li><NavLink to="/testprops/someValue">Test Props</NavLink></li>
                <li><NavLink to="/printquote">3. Print Quote</NavLink></li>
                <li><NavLink to="/domath">4. Do Math</NavLink></li>
                <li><NavLink to="/retirement">5. Retirement</NavLink></li>
                <li><NavLink to="/authenticate">Authenticate</NavLink></li>
                <li><NavLink to="/pomodoro">Pomodoro</NavLink></li>
              </ul>
          </div>
        </nav>
    }
}

//var Header = React.createClass({
//	render: function() {
//		return (
//        <nav className="navbar navbar-default">
//          <div className="container-fluid">
//              <Link to="app" className="navbar-brand">
//                <img src="images/pluralsight-logo.png" />
//              </Link>
//              <ul className="nav navbar-nav">
//                <li><Link to="app">Home</Link></li>
//                <li><Link to="authors">Authors</Link></li>
//                <li><Link to="pomodoro">Pomodoro</Link></li>
//                <li><Link to="actionItems">Action Items</Link></li>
//                <li><Link to="about">About</Link></li>
//              </ul>
//          </div>
//        </nav>
//		);
//	}
//});
//
//module.exports = Header;
//
