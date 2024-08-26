﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers.ConvertToModel
{
	public interface IEntityToModelConverter<TEntity, TModel>
	{
		TModel Convert(TEntity entity);
	}
}
