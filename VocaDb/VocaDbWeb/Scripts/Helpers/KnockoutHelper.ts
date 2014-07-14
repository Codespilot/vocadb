
module vdb.helpers {
	
	export class KnockoutHelper {
		
		public static stringEnum<T>(observable: KnockoutObservable<T>, enumType: any) {

			return ko.computed({
				read: () => enumType[observable()],
				write: (val: string) => observable(enumType[val])
			});

		}

	}

}