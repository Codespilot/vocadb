﻿interface KnockoutExtenders {

	// Parses the value as integer, converting it to null if it's not a valid number.
	parseInteger: KnockoutBindingHandler;

}

ko.extenders.parseInteger = (target) => {
	//create a writeable computed observable to intercept writes to our observable
	var result = ko.computed({
		read: target,  //always return the original observables value
		write: (newValue: any) => {
			var current = target(),
			valueToWrite = isNaN(newValue) || newValue == "" ? null : parseInt(newValue);

			//only write if it changed
			if (valueToWrite !== current) {
				target(valueToWrite);
			} else {
				//if the rounded value is the same, but a different value was written, force a notification for the current field
				if (newValue !== current) {
					target.notifySubscribers(valueToWrite);
				}
			}
		}
	}).extend({ notify: 'always' });

	//initialize with current value to make sure it is rounded appropriately
	result(target());

	//return the new computed observable
	return result;
};