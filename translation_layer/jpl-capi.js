/*globals console, window*/
define ('main',['require','jquery','ExtendedModel','api/snapshot/adapters/BackboneAdapter','sim-common/Detect','sim-common/VersionCheck','text!sim-common/templates/notSupported.html','api/snapshot/adapters/BackboneAdapter','api/snapshot/Controller','api/snapshot/Transporter','api/snapshot/SimCapiValue'],function(require){

    var $ = require('jquery');
    var $body;

    var ExtendedModel = require('ExtendedModel');
    var adapter = require('api/snapshot/adapters/BackboneAdapter').getInstance();

    var Capi = ExtendedModel.extend({});
    var capi;

    require('sim-common/Detect');
    require('sim-common/VersionCheck');

    var notSupportedTemplate = require('text!sim-common/templates/notSupported.html');
    var BackboneAdapter = require ('api/snapshot/adapters/BackboneAdapter').getInstance();
    var Controller = require('api/snapshot/Controller');
    var Transporter = require('api/snapshot/Transporter').getInstance();
    var Types = require('api/snapshot/SimCapiValue').TYPES;

	var initialized = false;

    window.receiveExposeFromRanger = function (name, type, value, allowedValues)
    {
        if (!initialized)
        {
            Controller.notifyOnReady();
            capi = new Capi();
            capi.on('change', onCapiChange);
            initialized = true;
        }

        var typeConverter = generateTypeConverter(type);
        value = typeConverter(value);

        var descObj = {};

        //console.log("allowed values: " + allowedValues);

        if (allowedValues)
        {
            descObj.allowedValues = allowedValues.map(typeConverter);
        }

        capi.set(name, value);
        adapter.expose(name, capi, descObj);
    };

	    window.receiveValueFromRanger = function (name, type, value) {
        if (initialized) {
            value = generateTypeConverter(type)(value);
            capi.set(name, value);
        }
    };

    window.UpdateScreenAtSPR = function (_func) 
    {
        Transporter.triggerCheck();
    }
	
    function storageSuccess() {
        console.log("Storage process successful for ");
        console.log(Transporter.getHandshake().authToken);
    };

    function storageError() {
        console.log("Storage process errored!");
    };
	
	function transmitDataToSim(value)
    {
        console.log(value.value.toString());
        // do a thing, preferrably where you take data from SPR and give it to Ranger
    }

    // Utilize the transporter for more permanent storage.
    // JOS: 12/9/2016
    window.storeRangerData = function (simId, key, value)
    {
        Transporter.setDataRequest(simId, key, value, storageSuccess, storageError);
    };

    window.getRangerData = function (simId, key)
    {
        Transporter.getDataRequest(simId, key, transmitDataToSim, storageError);
    };

    // TODO pregenerate for each type and cache
    function generateTypeConverter (type) {
        if (type === Types.NUMBER) {
            return function (value) {
                return parseFloat(value);
            };
        } else if (type === Types.BOOLEAN) {
            return function (value) {
                return value === "True" ? true : false;
            };
        }

        // for strings and arrays do nothing
        return function (value) { return value; };
    }

    function onCapiChange () {
        var changedAttributes = capi.changedAttributes();
        if (changedAttributes) {
            for (var i in changedAttributes)
            {
                console.log("setting " + i + " to " + changedAttributes[i]);
				
				if(typeof ranger_eclipse !== 'undefined')
				{
					//de nada
				}
				else
				{
					console.log("Ranger not found");
					return;
				}
				
				switch(i)
				{
					case "System.Show Shadow Labels":
						ranger_eclipse.update({"shadowLabelsEnabled": changedAttributes[i]});
						break;
					case "System.Camera FOV":
						ranger_eclipse.update({"setFOV": changedAttributes[i]});
						break;
					case "System.Add Location":
						// {id: "3", name: "Derpville", location:[34.0522, -118.2437]} <- format to Ranger
						// 3, Derpville, 34.0522, -118.2437 <- format from SPR
						var _message = changedAttributes[i].split(",");
						ranger_eclipse.update({"addLocation": {id: _message[0], name: _message[1], location:[_message[2], _message[3]]}});
						break;
				}
				
            }
        }
    }
	
	var ShowShadowLabels = false;
	var RangerFOV = 0.0;
	var RangerAddLocation = "";
	
	receiveExposeFromRanger("System.Show Shadow Labels", Boolean, ShowShadowLabels, null);
	receiveExposeFromRanger("System.Camera FOV", typeof RangerFOV, RangerFOV, null);
	receiveExposeFromRanger("System.Add Location", typeof RangerAddLocation, RangerAddLocation, null);
		
});