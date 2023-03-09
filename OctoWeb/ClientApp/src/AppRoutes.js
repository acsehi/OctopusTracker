import { Counter } from "./components/Counter";
import { EnergyConsumption } from "./components/EnergyConsumption";                                          
import { Home } from "./components/Home";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/counter',
        element: <Counter />
    },
    {
        path: '/fetch-energyUsage',
        element: <EnergyConsumption />
    }
];

export default AppRoutes;
