import React, { Component } from 'react';
import { setCookie, getCookie } from './Cookie';

function saveCredentials() {
    var userId = document.getElementById('username');
    var apiKey = document.getElementById('pass')

    setCookie("userid", userId);
    setCookie("apiKey", apiKey);
}

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <form onSubmit={saveCredentials()}>
                    <div>
                        <label for="username">Username:</label>
                        <input type="text" id="username" name="username" />
                    </div>

                    <div>
                        <label for="pass">API key</label>
                        <input type="password" id="pass" name="password"
                            minlength="8" required />
                    </div>
                    <input type="submit" value="Sign in" />
                </form>
            </div>
        );
    }
}
