import './main.css';
//
// import React from 'react';
// import ReactDOM from 'react-dom';
// import App from './components/App.jsx';
//
//
// ReactDOM.render(<div><App /></div>, document.getElementById('app'));

require('bootstrap-loader');
import React from 'react'
import { render } from 'react-dom'


import Routes from './core/Routes.jsx';

render((<Routes />
), document.getElementById('app'))
