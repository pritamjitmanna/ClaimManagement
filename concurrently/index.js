const concurrently=require("concurrently")
const path = require('path');

let commands = [];
let services = ["InsuranceCompany", "Insured", "IRDA", "Surveyor"];
let args = process.argv.slice(2); // Assuming args are passed as command line arguments

commands.push({
    command: `dotnet run --project ${path.join('..', 'Gateway.WebAPI', 'Gateway.WebAPI', 'Gateway.WebAPI.csproj')}`,
    name: "GateWay"
});

for (let service of args) {
    if (!services.includes(service)) {
        commands = [];
        console.log("Invalid Input. Only Options are ", services);
        process.exit(1);
    }
    commands.push({
        command: `dotnet run --project ${path.join('..', service, service, `${service}.csproj`)}`,
        name: service
    });
}

const {result}=concurrently(commands,{
    prefix:"name",
    KillOthers:["failure","success"],
    restartTries:2,
    prefixColors:['blue',"green",'red','yellow','cyan']
})

result.then(()=>console.log("All processes running successfully"),
(error)=>console.log("Error Occurred",error))