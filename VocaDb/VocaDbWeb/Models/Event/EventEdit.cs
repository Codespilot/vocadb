using System;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts.ReleaseEvents;
using System.Web.Mvc;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Event {

	public class EventEdit {

		public EventEdit() {
			Date = DateTime.Now;
			Description = string.Empty;
		}

		public EventEdit(ReleaseEventSeriesContract seriesContract)
			: this() {

			CopyNonEditableProperties(seriesContract);

		}

		public EventEdit(ReleaseEventDetailsContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			Date = contract.Date;
			Description = contract.Description;
			Id = contract.Id;
			Name = contract.Name;
			SeriesNumber = contract.SeriesNumber;

			CopyNonEditableProperties(contract);

		}

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		[CultureInvariantDateTimeModelBinder("Date")]
		public DateTime? Date { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public int? SeriesId { get; set; }

		public string SeriesName { get; set; }

		[Display(Name = "Series number")]
		public int SeriesNumber { get; set; }

		public void CopyNonEditableProperties(ReleaseEventDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AllSeries = contract.AllSeries;

			CopyNonEditableProperties(contract.Series);

		}

		public void CopyNonEditableProperties(ReleaseEventSeriesContract seriesContract) {

			if (seriesContract != null) {
				SeriesId = seriesContract.Id;
				SeriesName = seriesContract.Name;
			}

		}

		public ReleaseEventDetailsContract ToContract() {

			return new ReleaseEventDetailsContract {
				Date = this.Date,
				Description = this.Description ?? string.Empty,
				Id = this.Id,
				Name = this.Name,
				Series = (this.SeriesId != null ? new ReleaseEventSeriesContract { Id = this.SeriesId.Value, Name = this.SeriesName } : null), 
				SeriesNumber = this.SeriesNumber
			};

		}

	}

}