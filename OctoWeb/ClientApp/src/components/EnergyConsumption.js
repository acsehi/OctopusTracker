import React, { Component } from 'react';

export class EnergyConsumption extends Component {
    static displayName = EnergyConsumption.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
      this.populateEnergyUsageData();
  }

  static renderConsumptionTable(consumptions) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Consumption m3</th>
            <th>Unit cost</th>
            <th>Cost</th>
          </tr>
        </thead>
        <tbody>
          {consumptions.map(consumption =>
              <tr key={consumption.date}>
              <td>{consumption.date}</td>
              <td>{consumption.consumption}</td>
              <td>{consumption.unitCost}</td>
              <td>{consumption.cost}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
        : EnergyConsumption.renderConsumptionTable(this.state.energyUsage);

    return (
      <div>
        <h1 id="tabelLabel" >Energy consumption</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

    async populateEnergyUsageData() {    
    const response = await fetch('energyusage?userId=a&apiKep=b');
    const data = await response.json();
    this.setState({ energyUsage: data, loading: false });
  }
}
