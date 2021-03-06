﻿// Selectable numbered list item
// Catches onMouseOver, onMouseOut, onClick. Fires onItemClicked when clicked.
var SuggestionLineItem = React.createClass({
    // built in ability to verify type of each property
    // not required but very useful
    propTypes: {
        phrase: React.PropTypes.string, // phrase should be a string
        onItemClicked: React.PropTypes.func // onItemClicked event handler should be a function
    },

    // built in ablity to set initial state
    // Invoked once before the component is mounted. The return value will be used as the initial value of this.state.
    getInitialState: function () {
        return {
            isMouseOver: false          // Mouse is not over
        };
    },

    // Mouse over -> change state indicating that mouse is over
    handleMouseOver: function (e) {
        this.setState({ isMouseOver: true });
    },

    // Mouse out -> change state indicating that mouse is no longer over
    handleMouseOut: function (e) {
        this.setState({ isMouseOver: false });
    },

    // Mouse click -> bubble up and say that we are not selected
    handleClick: function (e) {
        // console.log(e.target.innerText);
        // as user types we want to load the suggestions once value is more than 2 letters
        if (e == null || e.target == null) return;
        // is event hadler assigned?
        // can't fire if not assigned. Exception will happen.
        if (this.props.onItemClicked == null) return;
        // fire
        this.props.onItemClicked({ Phrase: e.target.innerText });
        // change state indicating that mouse is no longer over
        this.setState({ isMouseOver: false });
    },

    render: function () {
        var styles = {
            paddingLeft: 2,
            paddingRight: 2,
            paddingTop: 2,
            paddingBottom: 2,
            backgroundColor: this.state.isMouseOver ? "#f5f5dc" : ""
        };
        return (
            <li style={styles} onMouseOver={this.handleMouseOver} onMouseOut={this.handleMouseOut}
                onClick={this.handleClick}>{this.props.phrase}</li>
        );
    }
});