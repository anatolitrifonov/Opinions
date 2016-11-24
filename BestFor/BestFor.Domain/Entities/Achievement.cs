using BestFor.Domain.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestFor.Domain.Entities
{
    /// <summary>
    /// List of achievements that user can get. This is basically just a lookup list.
    /// </summary>
    public class Achievement
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public virtual DateTime DateAdded { get; set; } = DateTime.Now;

        #region IObjectState implementation
        [NotMapped]
        public virtual ObjectState ObjectState { get; set; } = ObjectState.Added;
        #endregion

        [Required]
        public string Description { get; set; }
    }
}
