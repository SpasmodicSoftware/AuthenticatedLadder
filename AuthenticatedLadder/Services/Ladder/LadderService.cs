using System;
using System.Collections.Generic;
using AuthenticatedLadder.DomainModels;

namespace AuthenticatedLadder.Services.Ladder
{
    public class LadderService : ILadderService
    {
        private ILadderRepository _repository;
        public LadderService(ILadderRepository repository)
        {
            //TODO Injectare logger
            _repository = repository;
        }

        private bool isValidGetEntryForUserInput(string ladderId, string platform, string username)
        {
            return !string.IsNullOrEmpty(ladderId)
                && !string.IsNullOrEmpty(platform)
                && !string.IsNullOrEmpty(username);
        }

        public LadderEntry GetEntryForUser(string ladderId, string platform, string username)
        {
            if(!isValidGetEntryForUserInput(ladderId,platform,username))
            {
                return null;
            }
            LadderEntry result;
            try
            {
                result = _repository.GetEntryForUser(ladderId, platform, username);
            } 
            catch(Exception)
            {
                //TODO log
                throw;
            }
            return result;
        }

        public List<LadderEntry> GetTopEntries(string ladderId)
        {
            var result = new List<LadderEntry>();

            if (!string.IsNullOrEmpty(ladderId))
            {

                try
                {
                    result = _repository.GetTopEntries(ladderId);
                }
                catch (Exception)
                {
                    //TODO log error
                    throw;
                }
            }
            return result;
        }

        public LadderEntry Upsert(LadderEntry entry)
        {
            LadderEntry result = null;
            if(entry != null)
            {
                try
                {
                    result = _repository.Upsert(entry);
                }
                catch(Exception)
                {
                    //TODO logme
                    throw;
                }
            }
            return result;
        }
    }
}
